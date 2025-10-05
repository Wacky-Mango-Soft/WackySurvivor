# 🏝️ Wacky Survivor
> **Open World Survival Game**  
> Developed by **Wacky Mango Soft**

---

## 👥 Team
| Role | Name |
|------|------|
| 🧑‍💼 Team Leader | **김연태** |
| 🧑‍💻 Member | **박원재** |
| 🧑‍💻 Member | **김태현** |

📦 **GitHub Repository:**  
🔗 [https://github.com/Wacky-Mango-Soft/WSProject](https://github.com/Wacky-Mango-Soft/WSProject)

---

## 🌍 0. Real-Time Open World & Shader Utilization
> 실시간 월드 변화를 표현하고 다양한 셰이더를 활용한 오픈월드 구현

- 🌅 **Dynamic World Cycle:** Day → Afternoon → Night skybox blending  
- 🌙 **Adaptive Vision:** Reduced FOV (Field of View) at night  
- 🛏️ **Sleep Interaction:** Skipping time by resting on beds  
- 🎨 **ToonShader** applied for comic-style characters and environment  
  *(Character models are prototype placeholders)*

---

## 🎒 1. Inventory & Quick Slot System
> 익숙한 UI/UX로 조작이 편리한 인벤토리 및 퀵슬롯 시스템

- 🔄 Equipment switching & shortcut keys  
- 🗑️ Drag, drop, and discard functionalities  
- 🚫 Player control disabled during UI interaction  
- 💬 Item tooltips for contextual information

---

## 🌾 2. Gathering & Crafting System
> 다양한 자원 채집 및 제작 시스템 구현

- 🪓 **Gathering Types:**
  - Hunting  
  - Logging / Mining  
  - Collecting  
- 🏗️ **Crafting System:**
  - Item crafting  
  - Building  
- ✂️ **MeshCut** for model-based resource utilization

---

## 🗺️ 3. Map Design & Minimap Synchronization
> 터레인 크기와 미니맵 위치/회전을 실시간 동기화

- 📍 Real-time player tracking on minimap  
- 🧭 Dynamic rotation sync for accurate orientation

---

## 🏰 4. Dungeon Design & Optimization
> Blender로 제작된 던전의 인스턴스화 및 최적화

- 🧩 **Dungeon Manager** for scalable dungeon logic  
- ⚙️ Auto-instancing when player enters trigger radius  

---

## 🤖 5. NPC AI System
> NavMeshAgent & RayCast 기반의 행동 AI 구현

- 🧭 Multi-Agent system per NPC type  
- 👀 Detects player via **RayCast**, pursues or flees accordingly  
- 🧱 Avoids obstacles by recalculating shortest path  
- 👂 Sound-based detection for stealth gameplay  
- 🕵️ Stealth reduces detection radius *(planned)*  
- 🔊 Per-NPC **Audio Reverb Curve** for immersion  

---

## 🎥 6. 1st ↔ 3rd Person Camera Transition
> Multi-view camera for immersion and exploration

- 🔁 Camera switch via script system  
- 🧠 Head mesh hidden in first-person view  
- 🕹️ Avatar Mask & Animation Blending for smooth transitions  
- 🎯 Separate RayCast targeting per view type  

---

## 💾 7. Auto Save & Load System
> JSON 기반 세이브/로드 및 자동 저장 기능

- 📂 Save in JSON format  
- ⏱️ Auto-save linked to `TimeManager` property  
- ⚰️ Implements endpoint on player death  

---

## 🧩 8. Troubleshooting & Optimization
> 개발 과정 중 반복된 이슈와 해결 과정 요약

### 🧠 코드 및 구조적 문제
- 문서화 및 매니저/싱글톤 구조로 **중복 최소화**
- Enum + Serializer Attribute로 **문자열 오타 감소**

### 💾 협업 및 형상관리 문제
- `.gitignore`로 Asset Store 리소스 Push 방지  
- 별도 **Backup Repository** 운영  
- Unity Organization / Cloud 시스템 도입  

### 🪲 반복 루틴 버그
- Unity 공식문서 및 Coroutine 적극 활용  
- 조건문 분기 세분화  

### 🍏 macOS / Apple Silicon 호환성
- Unity / Apple 문서 및 커뮤니티 참조 해결  

---

## 🚧 9. Roadmap
> 향후 업데이트 및 개선 계획

- 🕵️ Stealth system  
- 🧘 Passive-type monsters  
- 🐾 Animal & monster taming  
- 🧠 Conversational NPC (LLM integration planned)  
- 📜 Storyline & dialogue system  
- ⚔️ Combat system (parry, skill unlocks by level)  
- 💬 Equipment visualization  
- ☁️ Weather & temperature system  
- 🧱 Custom modeling via Blender  
- 🎬 Cutscene & cinematic system  
- 🎨 Post-processing shaders  
- 🔁 Improved AutoSave cycle  
- 🌍 Multiplayer implementation  

---

## 🙏 Thank You
> *A project by **Wacky Mango Soft*** 🍋  
> “Explore. Survive. Build your world.”

---
