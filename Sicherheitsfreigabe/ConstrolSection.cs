namespace Sicherheitsfreigabe
{
    class ConstrolSection : Terminal
    {
        public void Challange(SafeteycardData card, int id)
        {
            card.GetCard(id);
        }

        public void Control(int id, SafeteycardData safetycard)
        {
            if (CanAccess(id, safetycard)) Open();
            Deny();
        }
    }
}
