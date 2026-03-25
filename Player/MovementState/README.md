# MovementState

플레이어 이동 상태(State Pattern)를 정의하고 상태 전이를 관리하는 스크립트 모음입니다.

## Scripts

* **MovementBaseState**
  모든 이동 상태의 공통 인터페이스를 정의하는 추상 클래스 

* **IdleState**
  플레이어가 아무 입력 없이 대기하는 상태 

* **WalkState**
  기본 이동(걷기) 상태를 처리하는 클래스 

* **RunState**
  달리기 상태 및 스태미너 기반 이동을 처리하는 클래스 

* **JumpState**
  점프 및 착지 후 상태 전이를 처리하는 클래스 

* **CrouchState**
  앉기 상태 및 앉은 상태 이동을 처리하는 클래스 
