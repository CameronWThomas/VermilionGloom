using System;

public class DragSecret : Secret
{
    private DragSecret()
    {
    }

    private DragSecret(DragSecret secret) : base(secret)
    { }

    public override SecretIconIdentifier Identifier => SecretIconIdentifier.Murder;

    protected override Secret Copy() => new DragSecret(this);

    protected override string CreateDescription()
    {
        return $"{SecretTarget.Name} was dragging the body of {AdditionalCharacter.Name} around";
    }

    public class Builder : SecretTypeBuilder<DragSecret>
    {
        private CharacterID _dragger = null;
        private CharacterID _dragged = null;

        public Builder(CharacterID secretOwner, SecretLevel secretLevel) : base(secretOwner, secretLevel) { }

        public Builder SetDragger(CharacterID dragger)
        {
            _dragger = dragger;
            return this;
        }

        public Builder SetDragged(CharacterID dragged)
        {
            _dragged = dragged;
            return this;
        }

        public override DragSecret Build()
        {
            ValidateNotNull(_dragger, nameof(_dragger));
            ValidateNotNull(_dragged, nameof(_dragged));

            var dragSecret = new DragSecret();
            Init(dragSecret, _dragger, _dragged);

            return dragSecret;
        }
    }
}