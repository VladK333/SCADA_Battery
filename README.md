# SCADA Project – External Battery Management

📘 A academic example from the course **Fundamentals of Safety-Critical Software in Power Systems**.
The project demonstrates a SCADA system for monitoring and controlling an external battery used for device charging, with charge/discharge simulation and command control via a dCom application.

---

## 📡 Communication
- **RTU slave address:** `10`
- **Transport protocol:** TCP
- **Port:** `25252`

---

## ⚙️ Configuration File (RtuCfg.txt)

The configuration file has been extended with the following parameters:

- `A` – scaling factor (default 1)
- `B` – offset (default 0)
- `LowAlarm` – lower capacity threshold (default 20)
- `EguMax` – maximum battery capacity (default 100)
- `AbnormalValue` – value for abnormal state (default = opposite of normal state)

👉 Normal states:
- T1, T2, T3, T4, I2 → **OFF**
- I1 → **ON**

---

## 🔄 Simulation Rules
1. If T1–T3 is active → capacity decreases by `-1%/s`.
2. If T4 is active → capacity decreases by `-3%/s`.
3. If I1 is active → capacity increases by `+2%/s`.
4. If I2 is active → capacity increases by `+3%/s`.
5. **LowAlarm logic:**
   - If `K < LowAlarm` → trigger alarm, deactivate T4 and activate I2.
6. **EguMax logic:**
   - If `K = EguMax` → deactivate the currently active charging source (I1 or I2).
7. The user may activate T1–T4 only if `K > LowAlarm`.

---