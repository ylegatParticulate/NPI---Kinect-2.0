##Collision Detection
#####Pastes of code around the internet implementing the collision

Pastes from [this site](https://social.msdn.microsoft.com/Forums/en-US/273f16b9-972d-4e40-9dd7-d04bcfd2d215/skeleton-stream-for-kinect-problems?forum=kinectsdk#d800b28b-4af6-43d0-8e23-216674543b0d).

######The result
```csharp
bubble.GetHitBoundary().Intersects(_player.RightHand.HitBox))
```
######Class Bubble - GetHitBoundary
```csharp
public Rectangle GetHitBoundary()
{
    var rect = new Rectangle(
        (int)(Location.X - (Origin.X * 0.25)),
        (int)(Location.Y - (Origin.Y * 0.25)),
        (int)(Bounds.Width * 0.25),
        (int)(Bounds.Height * 0.25));
    return rect;
}
```

#######Class BodyPart - Hitbox
```csharp
public Rectangle HitBox
{
    get { return GetHitBoundary(); }
}

public float HitBoxScale
{
    get; set;
}

private Rectangle GetHitBoundary()
{
    Rectangle rect;

    if (Math.Abs(HitBoxScale - 1.0f) < 0.01f)
    {
        rect = new Rectangle(
            (int) (Location.X - (Origin.X)),
            (int) (Location.Y - (Origin.Y)),
            Bounds.Width,
            Bounds.Height);
    }
    else
    {
        rect = new Rectangle(
            (int)((Location.X) - (Origin.X * HitBoxScale)),
            (int)((Location.Y) - (Origin.Y * HitBoxScale)),
            (int)(Bounds.Width * (HitBoxScale / 2)),
            (int)(Bounds.Height * (HitBoxScale / 2)));
    }
    return rect;
}
```

--
