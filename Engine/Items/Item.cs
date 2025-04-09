using CommonUtilities.Util;
using Engine.Persons;

namespace Engine.Items
{
	public abstract class Item
	{
		 protected History? _history;
       
        protected readonly Dictionary<Person, List<Operation>> Operations = [];
        internal History History() => _history;
       

        internal abstract ItemStatus Status();

        internal abstract void Be(object value);

        internal abstract void Reset();

        internal virtual void AddPerson(Person person, Role role)
        {   
            if(!Operations.ContainsKey(person)) Operations[person] = [];
            Operations[person] = role.Operations.Concat(Operations[person]).ToHashSet().ToList();
        }

        internal virtual void RemovePerson(Person person) => Operations.Remove(person);

        
		internal bool HasPerson(Person person) => Operations.Keys.Contains(person);
        internal virtual void History(History history) => _history = history;

        
		
        internal virtual void AddOperation(Person person,List<Operation> operations) => 
	        Operations[person] = operations;
        
		internal bool DoesAllow(Person person, Operation operation) => 
			Operations.ContainsKey(person) && Operations[person].Contains(operation);
		
		internal virtual bool Contains(Item desiredItem) => this == desiredItem;
		
		internal abstract void Accept(ChecklistVisitor visitor);
		
        internal virtual bool Replace(Item originalItem, Item newItem) => false;
        
        internal virtual void Simplify() {} // Ignore by default

		internal virtual bool Remove(Item item) => false;

		public Item I(int firstIndex, params int[] rest) {
			var results = rest.ToList();
			results.Insert(0, firstIndex);
			return I(results);
		}
		
		internal abstract Item I(List<int> indexes);
		
		internal abstract List<SimpleItem> ActiveItems();
    }

    public abstract class SimpleItem: Item
    {
       
       

    }

	public static class ItemExtensions
	{
		public static NotItem Not(this Item item) => new NotItem(item);
		
		public static OrItem Or(this Item item1, Item item2) => new OrItem(item1, item2);

		public static BooleanItem TrueFalse(this string question) => new BooleanItem(question);
		
		public static MultipleChoiceItem Choices(this string question, object firstChoice, params object[] choices)
		{
			if (choices.Any(choice => choice.GetType() != firstChoice.GetType())) throw new ArgumentException("All choices must be of the same type.");
			return new MultipleChoiceItem(question, firstChoice, choices: choices);
		}
    }
}
