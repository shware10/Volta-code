# Volta-code

Unity 기반 멀티플레이 게임 프로젝트로, 플레이어 상태 관리, UI 시스템, 카메라, 이펙트 등을 구성한 코드 구조입니다.

---

## Folder Structure

### Interface

게임 내 상태 변화 이벤트를 전달하기 위한 인터페이스 모음

* **IHpListener** : HP 변경 이벤트 수신
* **IQuickSlotChangedListener** : 퀵슬롯 변경 이벤트 수신
* **IQuickSlotSelectedListener** : 퀵슬롯 선택 변경 이벤트 수신
* **IStaminaListener** : 스태미너 변경 이벤트 수신
* **ISwapListener** : 슬롯 스왑 이벤트 수신

---

### Player

#### Camera

플레이어 카메라 및 시점 처리

* **AimStateManager** : 마우스 입력 기반 카메라 및 시점 회전 관리

#### MovementState

플레이어 이동 상태(State Pattern) 관리

* **MovementBaseState** : 상태 공통 인터페이스
* **IdleState** : 대기 상태
* **WalkState** : 걷기 상태
* **RunState** : 달리기 상태
* **JumpState** : 점프 상태
* **CrouchState** : 앉기 상태

#### StatusData

플레이어 상태 데이터 관리

* **HpManager** : 체력 및 사망 처리
* **StaminaManager** : 스태미너 관리
* **KillManager** : 킬 수 관리
* **Quickslot** : 인벤토리 및 슬롯 관리

---

### UI

#### InGameUI

게임 플레이 중 UI 처리

* **DraggableUI** : 아이템 드래그 처리
* **DroppableUI** : 슬롯 간 아이템 교환 처리
* **PointerButtonInteract** : 버튼 인터랙션 효과
* **QuickslotView** : 퀵슬롯 UI 갱신
* **SlotView** : 개별 슬롯 UI
* **StatusView** : 체력/스태미너 UI
* **Timer** : 게임 시작 카운트다운

#### OutGameUI

로그인 및 로비 UI

* **LoginPagePlayfab** : 로그인 / 회원가입 처리
* **LobbyPagePlayfab** : 계정 정보 및 리더보드 UI

---

### Effects

이펙트 및 파티클 관리

* **SetLightning** : 라이트닝 파티클 재생/정지 관리

---

## 특징

* 이벤트 기반 구조 (Listener 인터페이스 활용)
* 상태 패턴(State Pattern)을 이용한 이동 시스템
* Photon 기반 멀티플레이 처리
* PlayFab 연동 (로그인, 리더보드)
* UI와 로직 분리 (View / Manager 구조)

---
