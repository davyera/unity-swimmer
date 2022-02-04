using UnityEngine;

public class PlayerCharacter : MonoBehaviour, ICharacter {
    
    [SerializeField] private Transform _camera;

    private IStateMachine _state;

    public ICharacterInput Input { get; private set; }

    public IMovementProfile MovementProfile { get; private set; }
    
    public ITransformControl Transform { get; private set; } 

    public ICollisionDetector CollisionDetector { get; private set; }

    public Transform PerspectiveTransform { get => _camera; }

    void Start() {
        Input = GetComponent<ICharacterInput>();
        MovementProfile = GetComponent<IMovementProfile>();
        Transform = GetComponent<ITransformControl>();
        CollisionDetector = GetComponent<ICollisionDetector>();

        _state = new SimpleTransitionStateMachine();

        SwimmingState swimming = new SwimmingState(this);
        FloatingState floating = new FloatingState(this);
        CrawlingState crawling = new CrawlingState(this);
        HidingState hiding = new HidingState(this);

        _state.AddTransition(floating, swimming, () => HasMovementInput());
        _state.AddTransition(floating, crawling, () => IsGrounded());

        _state.AddTransition(swimming, floating, () => !HasMovementInput());
        _state.AddTransition(swimming, crawling, () => IsGrounded());

        _state.AddTransition(crawling, hiding, () => !HasFlatMovementInput() && IsDoneRotating());
        _state.AddTransition(crawling, swimming, () => ShouldSwim());

        _state.AddTransition(hiding, crawling, () => HasFlatMovementInput());
        _state.AddTransition(hiding, swimming, () => ShouldSwim());

        _state.InitState(swimming);
        _state.DebugLogTransitions();
    }

    private bool IsGrounded() => CollisionDetector.IsGrounded;

    private bool IsDoneRotating() =>
        Transform.ForwardLookVelocity.magnitude < 0.1 &&
        Transform.UpLookVelocity.magnitude < 0.1;

    private bool ShouldSwim() => 
        Input.JumpHeld && 
        !CollisionDetector.IsHeadBlocked || 
        !CollisionDetector.IsGroundWithinRange();

    private bool HasMovementInput() => HasFlatMovementInput() || HasVerticalMovementInput();
    
    private bool HasFlatMovementInput() => Mathf.Abs(Input.Horizontal) > 0 || Mathf.Abs(Input.Vertical) > 0;

    private bool HasVerticalMovementInput() => Input.DiveHeld || Input.JumpHeld;

    void Update() {
        _state.Tick();

        DrawDebug();
    }

    private void DrawDebug() {
    }
}
