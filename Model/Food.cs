namespace SnakeGame.Model
{
    public class FoodPool
    {
        private readonly List<Food> foods = [];

        public Food? PopFood()
        {
            if (foods.Count == 0) return null;

            Food item = foods.Last();
            foods.RemoveAt(foods.Count - 1);

            return item;
        }
        
        public void ReturnToPool(Food food) => foods.Add(food);
    }
    public class Food
    {
        public int? X { get; private set; }
        public int? Y { get; private set; }

        public bool IsActive => X.HasValue && Y.HasValue;

        public void Reset() // it should return to the pool of food objects instead of making a new obj after this method is called. 
        {
            this.X = null;
            this.Y = null;
        }
    }
}