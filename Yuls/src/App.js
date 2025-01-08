import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Page1 from './pages/page1'; // Import the Page1 component
import DynamicPage from './pages/DynamicPage'; // Import the DynamicPage component

function App() {
  return (
    <Router>
      <Routes>
        {/* Home route that renders Page 1 */}
        <Route path="/" element={<Page1 />} />
        {/* Dynamic route for Pages 2 to 15 */}
        <Route path="/page/:pageNumber" element={<DynamicPage />} />
        {/* Add more routes here as needed */}
      </Routes>
    </Router>
  );
}

export default App;
