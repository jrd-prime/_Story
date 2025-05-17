using R3;
using UnityEngine;

namespace _StoryGame.Infrastructure
{
    public interface IJInput
    {
        Observable<Vector2> TouchBegan { get; }
        Observable<Vector2> TouchMoved { get; }
        Observable<Vector2> TouchEnded { get; }
    }
}
