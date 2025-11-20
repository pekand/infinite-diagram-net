import sys
import time
import win32com.client

if len(sys.argv) < 3:
    print("Použitie: python skript.py <cesta_k_suboru> <riadok> <stlpec>")
    sys.exit(1)

filename = sys.argv[1]
line = int(sys.argv[2])
column = int(sys.argv[3])

# Použitie presného ID pre VS 2022
try:
    # Získanie bežiacej inštancie VS 2022
    dte = win32com.client.GetActiveObject("VisualStudio.DTE")
except Exception as e:
    print(f"Chyba: Nenašla sa bežiaca inštancia Visual Studio 2022. Spustite ho najskôr. {e}")
    sys.exit(1)

try:
    # 1. Aktivácia hlavného okna (je to metóda)
    dte.MainWindow.Activate()


except Exception as e:
    print(f"Vyskytla sa chyba počas automatizácie VS: {e}")