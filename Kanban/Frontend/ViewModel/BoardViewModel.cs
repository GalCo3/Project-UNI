using Frontend.Model;
using System;
using System.Windows;
using System.Windows.Media;

namespace Frontend.ViewModel

{
    public class BoardViewModel : NotifiableObject
    {

        //fields 

        private BoardModel _board;
        

        // constructor
        public BoardViewModel(BoardModel board) 
        {
            if (board != null)
            {
                Title = "Tasks for: " + board.BoardName;
                Board = board;
            }
            else
            {
                Title = "There are no boards for this user yet!";
            }
           
        }


        /// getters and setters
        public string Title { get; set; }
        public BoardModel Board { get { return _board; } set { _board = value; } }

    }  
}