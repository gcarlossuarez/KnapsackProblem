#!/bin/bash
set -e

APP_NAME="KnapsackProblem"
FRAMEWORK="net8.0"   # Ajustar según la versión de .NET
RUNTIMES=("osx-arm64" "osx-x64")  # Apple Silicon y Intel

for RUNTIME in "${RUNTIMES[@]}"; do
    echo "📦 Publicando para $RUNTIME ..."
    dotnet publish -c Release -r $RUNTIME --self-contained true /p:PublishSingleFile=true

    PUBLISH_DIR="bin/Release/$FRAMEWORK/$RUNTIME/publish"
    APP_DIR="$APP_NAME-$RUNTIME.app"
    DIST_DIR="dist-$RUNTIME"

    # Limpiar y crear estructura
    rm -rf "$APP_DIR" "$DIST_DIR"
    mkdir -p "$APP_DIR/Contents/MacOS"
    mkdir -p "$APP_DIR/Contents/Resources"
    mkdir -p "$DIST_DIR"

    # Copiar ejecutable y librerías
    cp "$PUBLISH_DIR/$APP_NAME" "$APP_DIR/Contents/MacOS/"
    cp "$PUBLISH_DIR/"*.dylib "$APP_DIR/Contents/MacOS/" 2>/dev/null || true
    chmod +x "$APP_DIR/Contents/MacOS/$APP_NAME"

    # Crear Info.plist
    cat > "$APP_DIR/Contents/Info.plist" <<EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN"
 "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleName</key>
    <string>$APP_NAME</string>
    <key>CFBundleDisplayName</key>
    <string>$APP_NAME</string>
    <key>CFBundleIdentifier</key>
    <string>com.GCSA.$APP_NAME.$RUNTIME</string>
    <key>CFBundleVersion</key>
    <string>1.0</string>
    <key>CFBundleExecutable</key>
    <string>$APP_NAME</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
</dict>
</plist>
EOF

    # Crear README.txt con instrucciones
    cat > "$DIST_DIR/README.txt" <<EOT
🚀 Instrucciones para ejecutar $APP_NAME en macOS

1. Descomprimir el archivo ZIP.
2. Verá la aplicación: $APP_NAME-$RUNTIME.app
3. La primera vez, Gatekeeper puede bloquearla porque no está firmada.
   - Haga clic derecho sobre la app -> "Abrir".
   - Aparecerá un mensaje de advertencia, pulsa "Abrir".
   - A partir de la segunda vez, se abrirá con doble clic normalmente.
4. Si el sistema aún bloquea la ejecución, puede habilitarla desde Terminal:
   chmod +x "$APP_NAME-$RUNTIME.app/Contents/MacOS/$APP_NAME"
   xattr -d com.apple.quarantine "$APP_NAME-$RUNTIME.app"

✅ Listo, ya puede jugar a la Mochila sin usar la Terminal.
EOT

    # Mover app al directorio de distribución y comprimir
    mv "$APP_DIR" "$DIST_DIR/"
    zip -r "$APP_NAME-$RUNTIME.zip" "$DIST_DIR"

    echo "✅ Paquete generado: $APP_NAME-$RUNTIME.zip"
done
