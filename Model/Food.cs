namespace SnakeGame.Model
{
    public class FoodPool
    {
        public FoodPool(int maxCapacity)
        {
            foodStack = new Stack<Food>(maxCapacity);
            this.Fill(maxCapacity);
            this.AllFoods = [.. foodStack];
        }
        private readonly List<Food> AllFoods = [];
        public IEnumerable<Food> ActiveFoods // API for the ViewModel
            => AllFoods.Where(f => f.IsActive == true);
        private readonly Stack<Food> foodStack;
        public Food Get(Coords newCoords)
        {
            if (foodStack.Count == 0)
                throw new InvalidOperationException("Cannot pop food since the pool is empty.");

            var food = foodStack.Pop();

            food.Activate();
            food.ChangeCoords(newCoords);

            return food;
        }
        public void ReturnToPool(Food food)
        {
            food.Dezactivate();
            foodStack.Push(food);
        }
        public void Fill(int MaxCapacity)
        {
            for (int i = foodStack.Count - 1; i < MaxCapacity; i++)
                foodStack.Push(new Food());
        }
    }
    public class Food()
    {
        public Coords Coords { get; private set; }
        public void ChangeCoords(Coords newCoords)
        => Coords = IsActive?
            newCoords 
            : throw new InvalidOperationException("cannot assign coords to inactive food.");
        public bool IsActive { get; private set; }
        public void Dezactivate()
        => IsActive = IsActive == true // if it was active before
            ? false                    // then desactivate
            : throw new InvalidOperationException("can't dezactivate food that's already inactive."); // else throw
        public void Activate()
        => IsActive = IsActive == false // if it was inactive before
            ? true                      // then activate
            : throw new InvalidOperationException("can't activate food that's already active."); // else throw
    }
}