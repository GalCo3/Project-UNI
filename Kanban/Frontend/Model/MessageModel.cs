//using Backend.Service.Objects;

namespace Frontend.Model
{
    public class MessageModel : NotifiableModelObject
    {
        //fields
        private int _id;
        private string _title;
        private string _body;
        

        //constructor 

        /// <summary>
        /// This is the constructor of the MEssageModel
        /// </summary>
        /// <param name="controller"> the backend controller</param>
        /// <param name="title"> title of the message</param>
        /// <param name="body">body of the message</param>
        public MessageModel(BackendController controller, string title, string body) : base(controller)
        {
            Title = title;
            Body = body;
        }


        //getters and setters
        public string Body
        {
            get => _body;
            set
            {
                this._body = value;
                RaisePropertyChanged("Body");
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                this._title = value;
                RaisePropertyChanged("Title");
            }
        }

        public int Id
        {
            get => _id;
            set
            {
                this._id = value;
                RaisePropertyChanged("Id");
            }
        }


    }
}
