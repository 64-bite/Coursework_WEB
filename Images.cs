using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Snake_game
{
    public static class Images
    {
        public readonly static ImageSource Empty = LoadImage("Empty.png");

        public readonly static ImageSource Body = LoadImage("Snake/Body.png");
        public readonly static ImageSource Head = LoadImage("Snake/Head.png");
        public readonly static ImageSource DeadBody = LoadImage("Snake/DeadBody.png");
        public readonly static ImageSource DeadHead = LoadImage("Snake/DeadHead.png");

        public readonly static ImageSource Blue_Head = LoadImage("Snake/Blue_Head.png");
        public readonly static ImageSource Blue_Body = LoadImage("Snake/Blue_Body.png");
        public readonly static ImageSource Blue_DeadBody = LoadImage("Snake/Blue_DeadBody.png");
        public readonly static ImageSource Blue_DeadHead = LoadImage("Snake/Blue_DeadHead.png");

        public readonly static ImageSource Red_Head = LoadImage("Snake/Red_Head.png");
        public readonly static ImageSource Red_Body = LoadImage("Snake/Red_Body.png");
        public readonly static ImageSource Red_DeadBody = LoadImage("Snake/Red_DeadBody.png");
        public readonly static ImageSource Red_DeadHead = LoadImage("Snake/Red_DeadHead.png");

        public readonly static ImageSource Orange_Head = LoadImage("Snake/Orange_Head.png");
        public readonly static ImageSource Orange_Body = LoadImage("Snake/Orange_Body.png");
        public readonly static ImageSource Orange_DeadBody = LoadImage("Snake/Orange_DeadBody.png");
        public readonly static ImageSource Orange_DeadHead = LoadImage("Snake/Orange_DeadHead.png");

        public readonly static ImageSource Yellow_Head = LoadImage("Snake/Yellow_Head.png");
        public readonly static ImageSource Yellow_Body = LoadImage("Snake/Yellow_Body.png");
        public readonly static ImageSource Yellow_DeadBody = LoadImage("Snake/Yellow_DeadBody.png");
        public readonly static ImageSource Yellow_DeadHead = LoadImage("Snake/Yellow_DeadHead.png");

        public readonly static ImageSource Purple_Head = LoadImage("Snake/Purple_Head.png");
        public readonly static ImageSource Purple_Body = LoadImage("Snake/Purple_Body.png");
        public readonly static ImageSource Purple_DeadBody = LoadImage("Snake/Purple_DeadBody.png");
        public readonly static ImageSource Purple_DeadHead = LoadImage("Snake/Purple_DeadHead.png");

        public readonly static ImageSource Food = LoadImage("Fruit/Food.png");
        public readonly static ImageSource Food_Orange = LoadImage("Fruit/Orange.png");
        public readonly static ImageSource Food_Kiwi = LoadImage("Fruit/Kiwi.png");
        public readonly static ImageSource Food_Apple = LoadImage("Fruit/Apple.png");
        public readonly static ImageSource Food_Cherry = LoadImage("Fruit/Cherry.png");

        public readonly static ImageSource Rand_NoBorder = LoadImage("NoBorder/Rand_NoBorder.png");
        public readonly static ImageSource Cherry_NoBorder = LoadImage("NoBorder/Cherry_NoBorder.png");
        public readonly static ImageSource Food_NoBorder = LoadImage("NoBorder/Food_NoBorder.png");
        public readonly static ImageSource Kiwi_NoBorder = LoadImage("NoBorder/Kiwi_NoBorder.png");
	    public readonly static ImageSource Apple_NoBorder = LoadImage("NoBorder/Apple_NoBorder.png");
        public readonly static ImageSource Orange_NoBorder = LoadImage("NoBorder/Orange_NoBorder.png");

	    public readonly static ImageSource RedNoBorder = LoadImage("NoBorder/RedNoBorder.png");
        public readonly static ImageSource YellowNoBorder = LoadImage("NoBorder/YellowNoBorder.png");
	    public readonly static ImageSource OrangeNoBorder = LoadImage("NoBorder/OrangeNoBorder.png");   
        public readonly static ImageSource GreenNoBorder = LoadImage("NoBorder/GreenNoBorder.png");
	    public readonly static ImageSource BlueNoBorder = LoadImage("NoBorder/BlueNoBorder.png");
        public readonly static ImageSource PurpleNoBorder = LoadImage("NoBorder/PurpleNoBorder.png");

        public readonly static ImageSource FogLight = LoadImage("Light.png");
        public readonly static ImageSource FogBlack = LoadImage("Black.png");

        public static int colorFruit = 0;
        public static int colorBody = 0;
        public static int colorHead = 0;
        public static void SetRandomFoodSkin()
        {
                Random rnd = new Random();
                colorFruit = rnd.Next(0, 5);
        }
        public static ImageSource ColorOfHead(int type)
        {
            switch (colorHead)
            {
                case 0:
                    if(type == 0)
                    {
                        return Red_Head;
                    } else
                    {
                         return RedNoBorder;
                    }
                case 1:
                    if(type == 0)
                        {
                         return Yellow_Head;
                    }
                    else
                    {
                         return YellowNoBorder;
                    }
                case 2:
                    if(type == 0)
                    {
                        return Orange_Head;
                    }
                    else
                    {
                         return OrangeNoBorder;
                    }
                case 3:
                    if(type == 0)
                    {
                        return Head;
                    }
                    else
                    {
                         return GreenNoBorder;
                    }
                case 4:
                    if(type == 0)
                    {
                        return Blue_Head;
                    }
                    else
                    {
                         return BlueNoBorder;
                    }
                case 5:
                    if(type == 0)
                    {
                        return Purple_Head;
                    }
                    else
                    {
                         return PurpleNoBorder;
                    }
                default:
                    return Head;
            }
        }
        public static ImageSource ColorOfBody()
        {
            switch (colorBody)
            {
                case 0:
                    return Red_Body;
                case 1:
                    return Yellow_Body;
                case 2:
                    return Orange_Body;
                case 3:
                    return Body;
                case 4:
                    return Blue_Body;
                case 5:
                    return Purple_Body;
                default:
                    return Body;
            }
        }
        public static ImageSource ColorOfDeadHead()
        {
            switch (colorHead)
            {
                case 0:
                    return Red_DeadHead;
                case 1:
                    return Yellow_DeadHead;
                case 2:
                    return Orange_DeadHead;
                case 3:
                    return Body;
                case 4:
                    return Blue_DeadHead;
                case 5:
                    return Purple_DeadHead;
                default:
                    return DeadHead;
            }

        }
        public static ImageSource ColorOfDeadBody()
        {
            switch (colorBody)
            {
                case 0:
                    return Red_DeadBody;
                case 1:
                    return Yellow_DeadBody;
                case 2:
                    return Orange_DeadBody;
                case 3:
                    return Body;
                case 4:
                    return Blue_DeadBody;
                case 5:
                    return Purple_DeadBody;
                default:
                    return DeadBody;
            }
        }
        public static ImageSource ColorOfFood(int type)
        {
            switch (colorFruit)
            {
                case 0:
                    if(type == 0)
                    {
                        return Food_Orange;
                    } else
                    {
                         return Orange_NoBorder;
                    }
                case 1:
                    if(type == 0)
                    {
                        return Food_Kiwi;
                    }
                    else
                    {
                        return Kiwi_NoBorder;
                    }
                case 2:
                    if(type == 0)
                    {
                        return Food_Apple;
                    }
                    else
                    {
                         return Apple_NoBorder;
                    }
                case 3:
                    if(type == 0)
                    {
                        return Food_Cherry;
                    }
                    else
                    {
                         return Cherry_NoBorder;
                    }
                case 4:
                    if(type == 0)
                    {
                        return Food;
                    }
                    else
                    {
                         return Food_NoBorder;
                    }       
                default:
                    return Food;
            }
        }
        private static ImageSource LoadImage(string fileName)
        {
            return new BitmapImage(new Uri($"Assets/{fileName}", UriKind.Relative));
        }

    }
}
