#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""分析 PromeRotation SDK 引用程序集的关键接口签名"""
import dnfile
import io, sys

SDK_DLL = r"C:\Users\1\.nuget\packages\promerotation.sdk.api15\0.1.0-preview.8\ref\net10.0-windows7.0\PromeRotation.dll"
OUT = r"E:\FF14_CPP\BlmAcr\PromeRotation_SDK_API.md"

pe = dnfile.dnPE(SDK_DLL)
md = pe.net.mdtables

def read_compressed_int(data, pos):
    b = data[pos]
    if (b & 0x80) == 0:
        return b, pos + 1
    elif (b & 0xC0) == 0x80:
        return ((b & 0x3F) << 8) | data[pos + 1], pos + 2
    elif (b & 0xE0) == 0xC0:
        return ((b & 0x1F) << 24) | (data[pos+1] << 16) | (data[pos+2] << 8) | data[pos+3], pos + 4
    raise ValueError("bad ci")

ET = {0x01:"void",0x02:"bool",0x03:"char",0x04:"sbyte",0x05:"byte",0x06:"short",0x07:"ushort",
      0x08:"int",0x09:"uint",0x0A:"long",0x0B:"ulong",0x0C:"float",0x0D:"double",0x0E:"string",
      0x10:"byref",0x13:"!!VAR",0x1E:"!MVAR",0x16:"typedref",0x18:"IntPtr",0x19:"UIntPtr",0x1C:"object"}

def tname(md, table_name, row):
    try:
        t = getattr(md, table_name).rows[row - 1]
        return f"{t.TypeNamespace}.{t.TypeName}"
    except Exception:
        return f"{table_name}#{row}"

def resolve(md, idx):
    tab, row = idx & 0x3, idx >> 2
    if tab == 0:
        return tname(md, 'TypeDef', row)
    if tab == 1:
        return tname(md, 'TypeRef', row)
    return f"TypeSpec#{row}"

def parse_type(md, data, pos):
    if pos >= len(data):
        return "?", pos
    b = data[pos]; pos += 1
    if b in ET:
        return ET[b], pos
    if b in (0x11, 0x12):
        v, pos = read_compressed_int(data, pos)
        return resolve(md, v), pos
    if b == 0x1D:
        t, pos = parse_type(md, data, pos)
        return t + "[]", pos
    if b == 0x14:
        t, pos = parse_type(md, data, pos)
        v, pos = read_compressed_int(data, pos)
        return f"{t}[{','.join(['?']*v)}]", pos
    if b == 0x15:
        kind = "valuetype" if data[pos] == 0x11 else "class"
        pos += 1
        v, pos = read_compressed_int(data, pos)
        base = resolve(md, v)
        argc, pos = read_compressed_int(data, pos)
        args = []
        for _ in range(argc):
            t, pos = parse_type(md, data, pos)
            args.append(t)
        return f"{kind} {base}<{', '.join(args)}>", pos
    if b in (0x0F, 0x10):
        t, pos = parse_type(md, data, pos)
        return ("byref " if b == 0x10 else "ptr ") + t, pos
    return f"<0x{b:02x}>", pos

def method_sig(md, blob):
    try:
        data = bytes(blob.value)
    except Exception:
        return "?"
    pos = 0
    cc = data[pos]; pos += 1
    generic = (cc & 0x10) != 0
    if generic:
        g, pos = read_compressed_int(data, pos)
    pc, pos = read_compressed_int(data, pos)
    ret, pos = parse_type(md, data, pos)
    params = []
    for _ in range(pc):
        t, pos = parse_type(md, data, pos)
        params.append(t)
    return f"({', '.join(params)}) -> {ret}" + (f" [gen={g}]" if generic else "")

def field_sig(md, blob):
    try:
        data = bytes(blob.value)
    except Exception:
        return "?"
    if not data:
        return "?"
    t, _ = parse_type(md, data, 1)
    return t

# 感兴趣的接口/类型（前缀匹配）
WANT = [
    "PromeRotation.Rotation.IRotation",
    "PromeRotation.Rotation.IRotationEventHandler",
    "PromeRotation.Rotation.IRotationLifecycle",
    "PromeRotation.Rotation.IRotationMeta",
    "PromeRotation.Rotation.IOpener",
    "PromeRotation.Rotation.RotationMetadataAttribute",
    "PromeRotation.Rotation.CountDownHandler",
    "PromeRotation.Rotation.IRotationContainer",
    "PromeRotation.Data.PAction",
    "PromeRotation.Data.ActionType",
    "PromeRotation.Data.AcrState",
    "PromeRotation.Timeline.Core.IJobNodeProvider",
    "PromeRotation.Timeline.Core.IJobNodeDescriptor",
    "PromeRotation.Core.ICore",
    "PromeRotation.Core.Core",
    "PromeRotation.Plugin",
    "PromeRotation.Helpers.ActionHelper",
    "PromeRotation.Managers.ActionQueueManager",
]

out = io.StringIO()
w = out.write
w("# PromeRotation SDK API15 关键接口签名\n\n")
w(f"> 来源: {SDK_DLL}\n\n")

methoddefs = list(md.MethodDef.rows)
typedefs = list(md.TypeDef.rows)
fields = list(md.Field.rows)

for row in typedefs:
    ns = str(row.TypeNamespace)
    tn = str(row.TypeName)
    full = f"{ns}.{tn}"
    matched = any(full.startswith(x) for x in WANT)
    if not matched:
        continue
    # 接口实现
    impls = []
    try:
        for ii in md.InterfaceImpl.rows:
            pass
    except Exception:
        pass
    w(f"## {full}\n")
    try:
        ext = row.Extends
        if ext is not None and hasattr(ext, 'row_index'):
            w(f"- 继承: {resolve(md, ext.row_index)}\n")
    except Exception:
        pass
    # 方法
    ml = row.MethodList
    ms = []
    if isinstance(ml, list):
        ms = [methoddefs[mi.row_index - 1] for mi in ml if mi.row_index - 1 < len(methoddefs)]
    else:
        start = ml.row_index - 1
        # 找下一个TypeDef定位范围
        nxt = None
        for r2 in typedefs:
            ml2 = r2.MethodList
            if not isinstance(ml2, list) and ml2.row_index - 1 > start:
                nxt = ml2.row_index - 1
                break
        if nxt is None:
            nxt = len(methoddefs)
        ms = methoddefs[start:nxt]
    if ms:
        w(f"- 方法 ({len(ms)}):\n")
        for m in ms:
            try:
                sig = method_sig(md, m.Signature)
            except Exception:
                sig = "?"
            try:
                stat = " static" if m.Flags.static else ""
            except Exception:
                stat = ""
            try:
                virt = " virtual" if m.Flags.virtual else ""
            except Exception:
                virt = ""
            w(f"  - {m.Name}{stat}{virt}: {sig}\n")
    # 字段
    fl = row.FieldList
    fs = []
    if isinstance(fl, list):
        fs = [fields[fi.row_index - 1] for fi in fl if fi.row_index - 1 < len(fields)]
    if fs:
        w(f"- 字段 ({len(fs)}):\n")
        for f_ in fs:
            try:
                fsig = field_sig(md, f_.Signature)
            except Exception:
                fsig = "?"
            w(f"  - {f_.Name}: {fsig}\n")
    w("\n")

result = out.getvalue()
with open(OUT, "w", encoding="utf-8") as f:
    f.write(result)
print("WROTE", OUT)
print("chars:", len(result))
