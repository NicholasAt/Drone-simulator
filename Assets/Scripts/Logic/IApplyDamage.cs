using System;

namespace Assets.Scripts.Logic
{
    public interface IApplyDamage
    {
        bool Died { get; }
        Action Happened { get; set; }
        void Hit(float damage);
    }
}