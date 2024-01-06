# -*- mode: python ; coding: utf-8 -*-

def get_mediapipe_path():
    import mediapipe
    mediapipe_path = mediapipe.__path__[0]
    return mediapipe_path


a = Analysis(
    ['hand.py'],
    pathex=['M:\\inzynierka_2\\PythonScripts'],
    binaries=[],
    datas=[
    ('M:\\PythonScriptsInterpreter\\Lib\\site-packages\\mediapipe\\python\\solutions\\*.*', 'mediapipe\\python\\solutions'),
    ('M:\\PythonScriptsInterpreter\\Lib\\site-packages\\mediapipe\\python\\*.*', 'mediapipe\\python'),
    ('M:\\inzynierka_2\\PythonScripts\\hand_classifier5.pth', '.'),
    ('M:\\inzynierka_2\\PythonScripts\\gesture_recognizer.task', '.')
    ],
    hiddenimports=['Torch'],
    hookspath=[],
    hooksconfig={},
    runtime_hooks=[],
    excludes=[],
    win_no_prefer_redirects=False,
    win_private_assemblies=False,
    noarchive=False,
)
pyz = PYZ(a.pure)

mediapipe_tree = Tree(get_mediapipe_path(), prefix='mediapipe', excludes=["*.pyc"])
a.datas += mediapipe_tree
a.binaries = filter(lambda x: 'mediapipe' not in x[0], a.binaries)


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