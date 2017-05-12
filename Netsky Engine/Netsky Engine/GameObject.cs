using Microsoft.Xna.Framework;

namespace Netsky_Engine
{
    public interface GameObject
    {
        void LoadContent();
        void UnloadContent();
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }
}
