namespace Frontend.Model
{
    public abstract class NotifiableModelObject : NotifiableObject
    {
        //fields
        public BackendController Controller { get; private set; }


        //constructor
        /// <summary>
        /// The following is a constructor for the NotifiableModelObject object
        /// </summary>
        /// <param name="controller">the BackendController that is responsible for communication with the backend</param>
        protected NotifiableModelObject(BackendController controller)
        {
            this.Controller = controller;
        }
    }
}
