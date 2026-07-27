@echo off
chcp 65001 >nul
echo Dosyalar birlestiriliyor...

(
    for /r %%f in (*.*) do (
        if /i not "%%~nxf"=="All.md" (
            echo ## Dosya: %%~nxf
            echo Konum: %%~dpf
            echo ```
            type "%%f"
            echo ```
            echo.
        )
    )
) > All.md

echo Islem tamamlandi: All.md olusturuldu.
pause