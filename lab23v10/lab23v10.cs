using System;
using System.Text;

namespace lab23
{
    public interface IAttacker { void Attack(); }
    public interface IHealer { void Heal(); }
    public interface ITalker { void TalkToNpc(); }

    public class WeaponSystem : IAttacker
    {
        public void Attack() => Console.WriteLine("[WeaponSystem]: Завдано удару!");
    }

    public class MedicalKit : IHealer
    {
        public void Heal() => Console.WriteLine("[MedicalKit]: Здоров'я відновлено.");
    }

    public class DialogueManager : ITalker
    {
        public void TalkToNpc() => Console.WriteLine("[DialogueManager]: Розмова з NPC.");
    }

    public class HeroAction
    {
        private readonly IAttacker _attacker;
        private readonly IHealer _healer;
        private readonly ITalker _talker;

        public HeroAction(IAttacker attacker, IHealer healer, ITalker talker)
        {
            _attacker = attacker;
            _healer = healer;
            _talker = talker;
        }

        public void Execute()
        {
            _attacker.Attack();
            _healer.Heal();
            _talker.TalkToNpc();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            IAttacker weapon = new WeaponSystem();
            IHealer medKit = new MedicalKit();
            ITalker dialogue = new DialogueManager();

            HeroAction hero = new HeroAction(weapon, medKit, dialogue);
            hero.Execute();
        }
    }
}