import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import ScratchCard from 'react-scratchcard';
import '../global.css'; // Use the global CSS file

const pageData = {
    2: { 
      title: "🥰", 
      content: "דבר ראשון את הדבר כאילו הכי מתוק שקיים", 
      image: require('../img/image2.JPG') 
    },
    3: { 
      title: "😊", 
      content: "יש לך חיוך ממכר שפשוט גורם לי לא משנה מה המצב רוח שלי לחייך והצחוק הנשי המתגלגל שלך בכלל, בעיקר שאת באמת באמת צוחקת ואת נשמעת כמו גופי :)", 
      image: require('../img/image3.JPG') 
    },
    4: { 
      title: "❤️", 
      content: "את רגישה קטנה שלי, ודברים מאוד נכנסים לך ללב", 
      image: require('../img/image4.JPG') // Replace with the correct image
    },
    5: { 
      title: "🌟", 
      content: "כמו שמאיה אומרת, בן אדם טהור שרוצה רק טוב לכולם ובעיקר לסובבים אותו ולאנשים החשובים לך", 
      image: require('../img/image5.JPG') // Replace with the correct image
    },
    6: { 
      title: "👑", 
      content: "מלכת היופי", 
      image: require('../img/image6.JPG') // Replace with the correct image
    },
    7: { 
      title: "🤗", 
      content: "בן אדם אכפתי, בודקת על הכל עם כולם שהכל טוב ובסדר, דואגת לאנשים שאת רואה שהם לא בטוב. חחחחח סליחה על התמונה לא מצאתי משהו אחר", 
      image: require('../img/image7.JPG') // Replace with the correct image
    },
    8: { 
      title: "🌞", 
      content: "תמיד אופטימית ותמיד מסתכלת על חצי הכוס המלאה, בדיוק סיפרתי לחברים שזה משהו שאני מאוד אוהב בך ושגם שינית אותי להיות בן אדם כזה יותר", 
      image: require('../img/image8.JPG') // Replace with the correct image
    },
    9: { 
      title: "🧘‍♀️", 
      content: "את אוהבת את הרוגע והשלווה שלך ואת תעשי המון בשביל לגרום לזה לקרות", 
      image: require('../img/image9.JPG') // Replace with the correct image
    },
    10: { 
      title: "🍲", 
      content: "מבשלת מדהים, אין מה להרחיב.", 
      image: require('../img/image10.jpg') // Replace with the correct image
    },
    11: { 
      title: "📞", 
      content: "מת על שיחת הדרך השגרתית ", 
      image: require('../img/image11.jpg') // Replace with the correct image
    },
    12: { 
      title: "💣", 
      content: "פצצה של העולמות, אמאלה", 
      image: require('../img/image12.JPG') // Replace with the correct image
    },
    13: { 
      title: "😴", 
      content: "נרדמת בשניה, אולי קצת מהר מידי...( תפסיקי לנחור)", 
      image: require('../img/image13.JPG') // Replace with the correct image
    },
    14: { 
      title: "🏃‍♀️", 
      content: "חשוב לך ספורט ואורך חיים בריא, תמיד תהפכי משהו לבריא ומזין יותר", 
      image: require('../img/image14.JPG') // Replace with the correct image
    },
    15: { 
      title: "🌄", 
      content: "אוהבת לטייל ובחוץ, נסיכת השמש שלי, תמיד תפתחי חלונות ותגרמי למקום להיות הרבה יותר מואר (גם מהשמש אבל בעיקר ממך)", 
      image: require('../img/image15.JPG') // Replace with the correct image
    },
    16: { 
      title: "😁", 
      content: "וכמובן הכי הכי חשוב איך אפשר לשכוח שכל ארוחה את דופקת חיוך שרואים איתו את השיני בינה שעוד לא צמחו כדי לשאול אם יש לך אוכל בשיניים", 
      image: require('../img/image16.JPG') // Replace with the correct image
    },
    17: { 
      title: "❤️", 
      content: "סתם חיים שלי באמת שאני יכול להוסיף עוד מלא מלא עמודים, את בן אדם מדהים שאני מעריץ מאריך (או מעריך לא יודע) ואני באמת באמת אוהב אותך על מי ומה שאת", 
      image: require('../img/image17.JPG') // Replace with the correct image
    },
};

function resizeImage(imageSrc, width, height) {
  return new Promise((resolve) => {
      const img = new Image();
      img.src = imageSrc;
      img.onload = () => {
          // Calculate the aspect ratio
          const aspectRatio = img.width / img.height;

          // Adjust width and height to maintain 3:4 aspect ratio
          let newWidth, newHeight;
          if (aspectRatio > 0.75) {
              newHeight = height;
              newWidth = height * aspectRatio;
          } else {
              newWidth = width;
              newHeight = width / aspectRatio;
          }

          const canvas = document.createElement('canvas');
          canvas.width = width;   // Desired width
          canvas.height = height; // Desired height
          const ctx = canvas.getContext('2d');
          
          // Draw the image centered on the canvas
          ctx.drawImage(
              img,
              (canvas.width - newWidth) / 2,
              (canvas.height - newHeight) / 2,
              newWidth,
              newHeight
          );

          resolve(canvas.toDataURL());
      };
  });
}



function DynamicPage() {
  const { pageNumber } = useParams();
  const navigate = useNavigate();
  const [isCardRevealed, setIsCardRevealed] = useState(false);
  const [scratchCardKey, setScratchCardKey] = useState(0);
  const [resizedImage, setResizedImage] = useState(null);

  useEffect(() => {
    setIsCardRevealed(false);
    setScratchCardKey(prevKey => prevKey + 1);
    
    const page = pageData[pageNumber];
    
    // Reset resizedImage when page changes
    setResizedImage(null);
    
    if (page && page.image) {
      resizeImage(page.image, 300, 400).then(resized => {
        setResizedImage(resized);
      });
    }
  }, [pageNumber]);

  const page = pageData[pageNumber];
  if (!page) return <div>THE END</div>;

  const settings = {
    width: 300,  // Set the width for the scratch card (3:4 aspect ratio)
    height: 400, // Set the height for the scratch card (3:4 aspect ratio)
    image: resizedImage, // Use the resized image
    finishPercent: 15,
    onComplete: () => {
      setIsCardRevealed(true);
    },
  };

  const nextPage = `/page/${parseInt(pageNumber) + 1}`;

  return (
    <div className="common-App">
      <header className="common-App-header">
        <h1>{page.title}</h1>
      </header>
      <div className="common-scratch-card-wrapper">
        {resizedImage && <ScratchCard key={scratchCardKey} {...settings} />}
      </div>
      {isCardRevealed && (
        <>
          <div className="common-content">
            <p>{page.content}</p>
          </div>
          <button className="common-next-button" onClick={() => navigate(nextPage)}>
            הבא
          </button>
        </>
      )}
    </div>
  );
}

export default DynamicPage;