🚀 Instrucciones para ejecutar KnapsackProblem en macOS

1. Descomprimir el archivo ZIP.
2. Verá la aplicación: KnapsackProblem-osx-x64.app
3. La primera vez, Gatekeeper puede bloquearla porque no está firmada.
   - Haga clic derecho sobre la app -> "Abrir".
   - Aparecerá un mensaje de advertencia, pulsa "Abrir".
   - A partir de la segunda vez, se abrirá con doble clic normalmente.
4. Si el sistema aún bloquea la ejecución, puede habilitarla desde Terminal:
   chmod +x "KnapsackProblem-osx-x64.app/Contents/MacOS/KnapsackProblem"
   xattr -d com.apple.quarantine "KnapsackProblem-osx-x64.app"

✅ Listo, ya puede jugar a la Mochila sin usar la Terminal.
