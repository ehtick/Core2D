# Core2D

[![CI](https://github.com/wieslawsoltes/Core2D/actions/workflows/build.yml/badge.svg)](https://github.com/wieslawsoltes/Core2D/actions/workflows/build.yml)

A multi-platform data driven 2D diagram editor.

## About

Core2D is a multi-platform application for making data driven 2D diagrams. 
The application has built-in wyswig vector graphics editor where you can bind data to shapes, 
share data across multiple documents, edit documents using layers. 
It supports exporting documents to many popular file formats like pdf, bitmaps and dxf. 
You can automate drawing and processing by using C# scripting.

![image](https://user-images.githubusercontent.com/2297442/131457859-94a2c5c3-f85c-4ac0-a7b0-ec07e86595b7.png)

## Supported Platforms

* `Windows 7 SP1 or higher`
* `Linux`
* `macOS`

## Usage

```
Core2D:
  A multi-platform data driven 2D diagram editor.

Usage:
  Core2D [options]

Options:
  -t, --theme <DefaultDark|DefaultLight|FluentDark|FluentLight>    Set application theme
  -s, --scripts <scripts>                                          The relative or absolute path to the script files
  -p, --project <project>                                          The relative or absolute path to the project file
  --repl                                                           Run scripting repl
  --useManagedSystemDialogs                                        Use managed system dialogs
  --useSkia                                                        Use Skia renderer
  --enableMultiTouch                                               Enable multi-touch [default: True]
  --useGpu                                                         Use Gpu [default: True]
  --allowEglInitialization                                         Allow EGL initialization [default: True]
  --useWgl                                                         Use Windows GL
  --useDeferredRendering                                           Use deferred rendering [default: True]
  --useWindowsUIComposition                                        Use Windows UI composition [default: True]
  --useDirectX11                                                   Use DirectX11 platform api
  --useHeadless                                                    Use headless
  --useHeadlessDrawing                                             Use headless drawing
  --useHeadlessVnc                                                 Use headless vnc
  --createHeadlessScreenshots                                      Create headless screenshots
  --vncHost <vncHost>                                              Vnc host
  --vncPort <vncPort>                                              Vnc port [default: 5901]
  --version                                                        Show version information
  -?, -h, --help                                                   Show help and usage information
```

## Resources

* [GitHub source code repository.](https://github.com/wieslawsoltes/Core2D)
* [Wiki.](https://github.com/wieslawsoltes/Core2D/wiki)

## JetBrains Rider

Core2D is developed using [JetBrains Rider](https://www.jetbrains.com/rider/).

## License

Core2D is licensed under the [MIT](LICENSE.TXT).
