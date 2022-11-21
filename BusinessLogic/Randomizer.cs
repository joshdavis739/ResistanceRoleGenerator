namespace BusinessLogic
{
    public class Randomizer<T>
    {
        public virtual IOrderedEnumerable<T> Randomize(ICollection<T> collection)
        {
            // TODO: ensure cryptographic security
            var randomizer = new Random();

            return collection.OrderBy(x => randomizer.Next());
        }
    }
}
