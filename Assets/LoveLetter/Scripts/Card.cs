namespace BBSL_LOVELETTER
{
    public enum eCARDVALUES
    {
        INVALID = 0,
        GUARD,
        PRIEST,
        BARON,
        HANDMAID,
        PRINCE,
        KING,
        COUNTESS,
        PRINCESS,
    }

    public class Card
    {
        private eCARDVALUES cardvalue = eCARDVALUES.INVALID;
        private int maxNum = 0;
        public Card(eCARDVALUES value)
        {
            cardvalue = value;
        }

        public Card(eCARDVALUES value, int numvalue)
        {
            cardvalue = value;
            maxNum = numvalue;
        }

        public void SetCardValue(eCARDVALUES value)
        {
            cardvalue = value;
        }

        public eCARDVALUES GetCardValue()
        {
            return cardvalue;
        }

        public int GetMaxCard()
        {
            return maxNum;
        }
    }
}