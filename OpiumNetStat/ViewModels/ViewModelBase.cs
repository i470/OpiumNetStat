using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;

namespace OpiumNetStat.ViewModels
{
    public abstract class ViewModelBase : BindableBase, IDestructible
    {
        

        protected ViewModelBase()
        {
            
        }

        public virtual void Destroy()
        {

        }
    }
}