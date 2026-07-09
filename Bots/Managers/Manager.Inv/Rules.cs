

namespace ManagerInventory
{
    public class Rule
    {

        public string Name = "";
        public string Lql = "";
        public string Hql = "";

        public Rule(string Name, string Lql, string Hql)
        {
            this.Name = Name;
            this.Lql = Lql;
            this.Hql = Hql;
        }

    }
}
