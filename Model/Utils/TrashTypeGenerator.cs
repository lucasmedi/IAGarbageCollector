using System;

namespace Model.Utils
{
    public class TrashTypeGenerator
    {
        private static int index = 0;
        private static int trashTypes = 4;

        public static TrashType getRandomTrashType()
        {
            var rnd = new Random(trashTypes);

            int index = (int)(rnd.Next() * trashTypes);
            return (TrashType)trashTypes;
        }

        public static TrashType next()
        {
            if (index < trashTypes - 1)
            {
                index++;
            }
            else
            {
                index = 0;
            }

            return (TrashType)trashTypes;
        }

        public static string getTrashCanIcon(TrashType trashType)
        {
            switch (trashType)
            {
                case TrashType.GLASS:
                    return "img/trashCanEmptyGreen.png";
                case TrashType.PAPER:
                    return "img/trashCanEmptyBlue.png";
                case TrashType.METAL:
                    return "img/trashCanEmptyYellow.png";
                case TrashType.PLASTIC:
                    return "img/trashCanEmptyRed.png";
                default:
                    break;
            }

            return string.Empty;
        }

        public static string getTrashIcon(TrashType trashType)
        {
            switch (trashType)
            {
                case TrashType.GLASS:
                    return "img/trashGreen.png";
                case TrashType.PAPER:
                    return "img/trashBlue.png";
                case TrashType.METAL:
                    return "img/trashYellow.png";
                case TrashType.PLASTIC:
                    return "img/trashRed.png";
                default:
                    break;
            }

            return string.Empty;
        }

        public static void setIndex(int index)
        {
            TrashTypeGenerator.index = index;
        }
    }
}