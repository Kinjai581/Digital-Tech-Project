using System;
using Microsoft.Xna.Framework.Content;

namespace Test;

public static class Globals {

    public static float Time {get; private set; }
    public static ContentManager Content {get; private set; }
    //public static SpriteBatch SpriteBatch {get; private set; }

}

//public static void Update (Gametime gt){
    //Time = (float)gt.ElaspedGameTime.TotalSeconds;
//}