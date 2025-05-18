using R3;
using UnityEngine;

namespace _StoryGame.Infrastructure.Input.Interfaces
{
    public interface IJInput
    {
        Observable<Vector2> TouchBegan { get; }
        Observable<Vector2> TouchMoved { get; }
        Observable<Vector2> TouchEnded { get; }
    }
}
