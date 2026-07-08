@echo off
chcp 65001 >nul
echo Dosyalar birlestiriliyor...
(for /r %%f in (*.cs) do (
    echo ## Dosya: %%~nxf
    echo ```csharp
    type "%%f"
    echo ```
    echo.
)) > All.md
echo Islem tamamlandi: All.md olusturuldu.
pause