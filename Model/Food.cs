namespace SnakeGame.Model
{
    public class FoodPool
    {
        public FoodPool(int maxCapacity)
        {
            foodStack = new Stack<Food>(maxCapacity);
            this.Fill();
        }
        private readonly Stack<Food> foodStack;
        public Food PopAndAssign((int row, int col) newCoords)
        {
            if (foodStack.Count == 0)
                throw new InvalidOperationException("Cannot pop food since the pool is empty.");

            var food = foodStack.Pop();

            return food;
        }
        public void ReturnToPool(Food food)
        {
            food.Reset();
            foodStack.Push(food);
        }
        public void Reset() 
        {
            foodStack.Clear();
            this.Fill();
        }
        public void Fill()
        {
            for (int i = foodStack.Count - 1; i < foodStack.Capacity; i++)
                foodStack.Push(new Food());
        }
    }
    public class Food
    {
        public bool IsActive { get; private set; }
        public void Reset() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}