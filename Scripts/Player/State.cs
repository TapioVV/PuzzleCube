using Godot;
using System;

public abstract partial class State : Node2D
{
    //public BoxCollider2D boxCollider;

    public Player player;
    //public Animator animator;
    //public SpriteRenderer spriteRenderer;

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update(float deltaf);
    
    public virtual void AfterMoveAndSlideUpdate(float deltaf)
    {

    }

}
