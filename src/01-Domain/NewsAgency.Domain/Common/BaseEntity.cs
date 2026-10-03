namespace NewsAgency.Domain.Common;

public class BaseEntity<TId>
{

    public TId Id { get; protected set; }



    public bool Equals(BaseEntity<TId>? other) => this == other;
    public override bool Equals(object? obj)=>
         obj is BaseEntity<TId> otherObject && Id.Equals(otherObject.Id);

    public override int GetHashCode() => Id.GetHashCode();
    public static bool operator ==(BaseEntity<TId> left, BaseEntity<TId> right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals((object) right);
    }

    public static bool operator !=(BaseEntity<TId> left, BaseEntity<TId> right)
        => !(right == left);

}


public class BaseEntity : BaseEntity<long>
{

}
