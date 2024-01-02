# -*- mode: python ; coding: utf-8 -*-

a = Analysis(
    ['hand.py'],
    pathex=['C:\\Users\\versi\\MageHand-UnityGame\\PythonScripts'],
    binaries=[],
    datas=[
    ('C:\\Users\\versi\\anaconda3\\envs\\hand\\Lib\\site-packages\\mediapipe\\python\\solutions\\*.*', 'mediapipe\\python\\solutions'),
    ('C:\\Users\\versi\\MageHand-UnityGame\\PythonScripts\\gesture_recognizer.task', '.')
    ],
    hiddenimports=[],
    hookspath=[],
    hooksconfig={},
    runtime_hooks=[],
    excludes=[],
    win_no_prefer_redirects=False,
    win_private_assemblies=False,
    noarchive=False,
)
pyz = PYZ(a.pure)

exe = EXE(
    pyz,
    a.scripts,
    [],
    exclude_binaries=True,
    name='hand',
    debug=False,
    bootloader_ignore_signals=False,
    strip=False,
    upx=True,
    console=True,
    disable_windowed_traceback=False,
    argv_emulation=False,
    target_arch=None,
    codesign_identity=None,
    entitlements_file=None,
)
coll = COLLECT(
    exe,
    a.binaries,
    a.datas,
    strip=False,
    upx=True,
    upx_exclude=[],
    name='hand',
)