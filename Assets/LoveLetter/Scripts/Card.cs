namespace BBSL_LOVELETTER
{
    public enum eCardValues
    {
        INVALID = -1,
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
        private eCardValues cardvalue = eCardValues.INVALID;
        private int maxNum = 0;
        public Card(eCardValues value)
        {
            cardvalue = value;
        }

        public Card(eCardValues value, int numvalue)
        {
            cardvalue = value;
            maxNum = numvalue;
        }

        public void SetCardValue(eCardValues value)
        {
            cardvalue = value;
        }

        public eCardValues GetCardValue()
        {
            return cardvalue;
        }

        public int GetMaxCard()
        {
            return maxNum;
        }
    }
}