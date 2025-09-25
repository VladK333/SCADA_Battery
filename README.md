# SCADA Projekat – Upravljanje eksternom baterijom

📘 Školski primer iz predmeta **Osnove softvera sa kritičnim odzivom u elektroenergetskim sistemima**.  
Projekat prikazuje SCADA sistem za nadzor i upravljanje eksternom baterijom za punjenje uređaja, sa simulacijom punjenja/pražnjenja i komandovanjem preko dCom aplikacije.

---

## 📡 Komunikacija
- **RTU slave adresa:** `10`  
- **Transportni protokol:** TCP  
- **Port:** `25252`

---

## ⚙️ Konfiguracioni fajl (RtuCfg.txt)

Konfiguraciona datoteka proširena je parametrima:

- `A` – faktor skaliranja (default 1)  
- `B` – odstupanje (default 0)  
- `LowAlarm` – niža granična vrednost kapaciteta (default 20)  
- `EguMax` – maksimalan kapacitet baterije (default 100)  
- `AbnormalValue` – vrednost za abnormalno stanje (default = suprotno od normalnog stanja)

👉 Normalna stanja:
- T1, T2, T3, T4, I2 → **OFF**
- I1 → **ON**

---

## 🔄 Pravila simulacije
1. Ako je T1–T3 aktivan → kapacitet se smanjuje za `-1%/s`.  
2. Ako je T4 aktivan → kapacitet se smanjuje za `-3%/s`.  
3. Ako je I1 aktivan → kapacitet se povećava za `+2%/s`.  
4. Ako je I2 aktivan → kapacitet se povećava za `+3%/s`.  
5. **LowAlarm logika:**  
   - Ako `K < LowAlarm` → javi alarm, isključi T4 i uključi I2.  
6. **EguMax logika:**  
   - Ako `K = EguMax` → isključi trenutno aktivno punjenje (I1 ili I2)  
7. Korisnik može uključivati T1–T4 samo ako je `K > LowAlarm`.

---
