#/bin/bash:

# Para Windows
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true

# Para Linux
dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishSingleFile=true

# Para Mackintosh
./publicar_proyecto_para_mackintosh.sh

# Instala Avalonia.Package que automatiza el proceso de crear .msi, .deb, .rpm y .dmg
#dotnet tool install --global Avalonia.Package


# Genera instalador para Windows
#dotnet tool install --global Avalonia.Package

# Genera instaladores msi
#dotnet avalonia-package -r win-x64
#dotnet avalonia-package -r linux-x64
#dotnet avalonia-package -r osx-x64


