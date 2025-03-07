using Engine.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Items
{
	public class BooleanItemVisitor : ChecklistVisitor
	{
		private Dictionary<string, bool?> _answers = [];
		public BooleanItemVisitor(Checklist checklist)
		{
			checklist.Accept(this);

		}

		public object? value(string question)
		{
			if (!_answers.ContainsKey(question)) throw new ArgumentException("No Such Question.");
			return _answers[question];
		}

		public void Visit(BooleanItem item,string question, bool? value, Dictionary<Person, List<Operation>> operations)
		{
			_answers[question]= value;
		}

	}

}
