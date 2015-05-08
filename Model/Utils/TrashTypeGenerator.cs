using System;

namespace Model.Utils
{
    public class TrashTypeGenerator
    {
        private static Random rnd;

        private static int index = 0;
        private static int trashTypes = 4;

        public static TrashType getRandomTrashType()
        {
            rnd = rnd ?? new Random();
            int index = (int)rnd.Next(trashTypes);
            return (TrashType)index;
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

            return (TrashType)index;
        }

        public static string getTrashCanIcon(TrashType trashType)
        {
            switch (trashType)
            {
                case TrashType.GLASS:
                    return "Content/Images/trashCanEmptyGreen.png";
                case TrashType.PAPER:
                    return "Content/Images/trashCanEmptyBlue.png";
                case TrashType.METAL:
                    return "Content/Images/trashCanEmptyYellow.png";
                case TrashType.PLASTIC:
                    return "Content/Images/trashCanEmptyRed.png";
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
                    return "Content/Images/trashGreen.png";
                case TrashType.PAPER:
                    return "Content/Images/trashBlue.png";
                case TrashType.METAL:
                    return "Content/Images/trashYellow.png";
                case TrashType.PLASTIC:
                    return "Content/Images/trashRed.png";
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