using System;

namespace Assets.Scripts.Logic
{
    public interface IApplyDamage
    {
        Action Happened { get; set; }
        void Hit(float damage);
    }
}