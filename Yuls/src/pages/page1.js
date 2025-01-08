import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import '../global.css'; // Use the global CSS file

function Page1() {
  const [isChecked, setIsChecked] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [canCheck, setCanCheck] = useState(false);
  const navigate = useNavigate();

  const handleCheckboxChange = () => {
    if (canCheck) {
      setIsChecked(!isChecked);
    }
  };

  const handleOpenModal = () => {
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setCanCheck(true); // Enable the checkbox after closing the modal
  };

  const handleClick = () => {
    alert('אם לא הבנת זה כאילו מה שאמרתי לך שאני עובד עליו..........');
    navigate('/page/2');
  }

  return (
    <div className="page1-App">
      <header className="page1-App-header">
        <h1>ברוכה הבאה שמנדוזה !</h1>
      </header>
      <div className="page1-content">
        <label className="page1-label">
          <input
            type="checkbox"
            checked={isChecked}
            onChange={handleCheckboxChange}
            disabled={!canCheck} // Disable the checkbox until the terms have been viewed
          />
          אני מאשרת שקראתי את <span className="page1-terms-link" onClick={handleOpenModal}>תקנון האתר</span>
        </label>
        <button className="page1-button" disabled={!isChecked} onClick={handleClick}>
          הבא
        </button>
      </div>

      {/* Modal Window */}
      {isModalOpen && (
        <div className="page1-modal">
          <div className="page1-modal-content">
            <h2>תקנון האתר</h2>
            <p>
              כל הבא למטה מחויב רק אם יולי תישאר אותו הדבר אותו הדבר:
            </p>
            <ul>
              <li>שמנה</li>
              <li>יפה</li>
              <li>מסריחה</li>
            </ul>
            <p>תודה ויום טוב</p>
            <p>את צריכה לגרד !!</p>
            <button onClick={handleCloseModal}>סגור</button>
          </div>
        </div>
      )}
    </div>
  );
}

export default Page1;
