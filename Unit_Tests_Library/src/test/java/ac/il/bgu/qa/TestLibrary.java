package ac.il.bgu.qa;

import ac.il.bgu.qa.errors.*;
import ac.il.bgu.qa.services.*;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

import org.junit.jupiter.api.*;
import org.junit.jupiter.api.extension.ExtendWith;
import org.junit.jupiter.params.*;
import org.junit.jupiter.params.provider.CsvSource;
import org.junit.jupiter.params.provider.EmptySource;
import org.junit.jupiter.params.provider.NullSource;
import org.junit.jupiter.params.provider.ValueSource;
import org.junit.jupiter.api.Nested;
import org.mockito.*;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

public class TestLibrary {

    @Mock
    private DatabaseService databaseService;
    @Mock
    private NotificationService notificationService;
    @Mock
    private ReviewService reviewService;

    private Library library;

    @Mock
    private static User userMock;

    @Mock
    private static Book bookMock;

    @Spy
    private static List<String> reviews;


    @BeforeEach
    void setUp() {
        MockitoAnnotations.openMocks(this);
        reviews = spy(new ArrayList<>());
        library = new Library(databaseService, reviewService);


        when(userMock.getId()).thenReturn("123456789012");
        when(bookMock.getISBN()).thenReturn("978-3-16-148410-0"); // this is from wikipedia
    }

    @Test
    void givenNullBook_whenAddBook_thenThrowException() {
        assertThrows(IllegalArgumentException.class, () -> library.addBook(null));
        verify(databaseService, never()).addBook(bookMock.getISBN(), bookMock);
    }

    @ParameterizedTest
    @CsvSource({
            "1234567890123",
            "123-456789002",
            "123-000000000000",
            "123-0000000vb00A",
            "123-ABCDE-FGHIJ",
            "123-12345-67890-12345",
            "123-4567",
            "123-LALA-LO"

    })
    @NullSource
    void givenNotValidISBN_whenAddBook_IllegalArgumentException(String isbn) {
        Book book = mock(Book.class);
        when(book.getISBN()).thenReturn(isbn);
        when(book.getTitle()).thenReturn("The Zookeeper's Wife");
        when(book.getAuthor()).thenReturn("Diane Ackerman");
        assertThrows(IllegalArgumentException.class, () -> library.addBook(book));
        verify(databaseService, never()).addBook(isbn, bookMock);
    }

    @ParameterizedTest
    @NullSource
    @ValueSource(strings = {""})
    void givenNull0rEmptyTitle_whenAddBook_thenThrowIllegalArgumentException(String title) {
        Book book = mock(Book.class);
        when(book.getISBN()).thenReturn("978-3-16-148410-0");
        when(book.getTitle()).thenReturn(title);
        when(book.getAuthor()).thenReturn("Diane Ackerman");
        assertThrows(IllegalArgumentException.class, () -> library.addBook(book));
        verify(databaseService, never()).addBook(bookMock.getISBN(), bookMock);
    }

    @ParameterizedTest
    @NullSource
    @ValueSource(strings = {"", "Di@na", " !Diana", "Habba!@,", " ", "--Diana", "dddd//",
            "B7",
            "1B",
            "Diane--Ackerman"})
    void givenInvalidAuthorName_whenAddBook_thenThrowIllegalArgumentException(String author) {
        Book book = mock(Book.class);
        when(book.getISBN()).thenReturn("978-3-16-148410-0");
        when(book.getTitle()).thenReturn("The Zookeeper's Wife");
        when(book.getAuthor()).thenReturn(author);
        assertThrows(IllegalArgumentException.class, () -> library.addBook(book));
        verify(databaseService, never()).addBook(bookMock.getISBN(), bookMock);
    }

    @Test
    void givenBorrowedBook_whenAddBook_thenThrowIllegalArgumentException() {
        when(bookMock.isBorrowed()).thenReturn(true);
        assertThrows(IllegalArgumentException.class, () -> library.addBook(bookMock));
        verify(databaseService, never()).addBook(bookMock.getISBN(), bookMock);
    }

    @Test
    void givenExistingBook_whenAddBook_thenThrowIllegalArgumentException() {
        when(databaseService.getBookByISBN(bookMock.getISBN())).thenReturn(bookMock);
        assertThrows(IllegalArgumentException.class, () -> library.addBook(bookMock));
        verify(databaseService, never()).addBook(bookMock.getISBN(), bookMock);
    }


    @Test
    void givenValidBook_whenAddBook_thenDoesNotThrow() {
        when(bookMock.getTitle()).thenReturn("The Zookeeper's Wife");
        when(bookMock.getAuthor()).thenReturn("Diane Ackerman");
        assertDoesNotThrow(() -> library.addBook(bookMock));
        verify(databaseService, atLeastOnce()).addBook(bookMock.getISBN(), bookMock);
    }

    @Test
    void givenBorrowedBook_whenBorrowBook_thenThrowBookAlreadyBorrowedException() {
        when(bookMock.isBorrowed()).thenReturn(true);
        when(databaseService.getBookByISBN(bookMock.getISBN())).thenReturn(bookMock);
        when(databaseService.getUserById(userMock.getId())).thenReturn(userMock);
        assertThrows(BookAlreadyBorrowedException.class, () -> library.borrowBook(bookMock.getISBN(), userMock.getId()));
        verify(bookMock, never()).borrow();
        verify(bookMock, atLeastOnce()).isBorrowed();
        verify(databaseService, never()).borrowBook(bookMock.getISBN(), userMock.getId());
    }

    @ParameterizedTest
    @ValueSource(strings = {
            "1234567890123",
            "123-456789002",
            "123-000000000000",
            "123-0000000vb00A",
            "123-ABCDE-FGHIJ",
            "123-12345-67890-12345",
            "123-4567",
            "123-LALA-LO"

    })
    @NullSource
    void givenNotValidISBN_whenBorrowBook_thenThrowIllegalArgumentException(String isbn) {
        when(bookMock.getISBN()).thenReturn(isbn);
        assertThrows(IllegalArgumentException.class, () -> library.borrowBook(bookMock.getISBN(), userMock.getId()));
        verify(bookMock, never()).borrow();
        verify(bookMock, never()).isBorrowed();
        verify(databaseService, never()).borrowBook(bookMock.getISBN(), userMock.getId());
    }

    @Test
    void givenNotFoundBook_whenBorrowBook_thenThrowBookNotFoundException() {
        when(databaseService.getBookByISBN(bookMock.getISBN())).thenReturn(null);
        assertThrows(BookNotFoundException.class, () -> library.borrowBook(bookMock.getISBN(), userMock.getId()));
        verify(bookMock, never()).borrow();
        verify(bookMock, never()).isBorrowed();
        verify(databaseService, never()).borrowBook(bookMock.getISBN(), userMock.getId());
    }


    @ParameterizedTest
    @ValueSource(strings = {
            "1234567890",
            "123-4567002",
            "0000",
            "123-0000000vb00A",
            "123ABCDFGHIJ",
            "123-12367890-12345",
            "123-4567",
            "123LALA-LO",
            "",
            " "

    })
    @NullSource
    void givenNotValidUserId_whenBorrowBook_thenThrowIllegalArgumentException(String id) {
        when(userMock.getId()).thenReturn(id);
        when(databaseService.getBookByISBN(bookMock.getISBN())).thenReturn(bookMock);
        assertThrows(IllegalArgumentException.class, () -> library.borrowBook(bookMock.getISBN(), userMock.getId()));
        verify(bookMock, never()).borrow();
        verify(bookMock, never()).isBorrowed();
        verify(databaseService, never()).borrowBook(bookMock.getISBN(), userMock.getId());
    }

    @Test
    void whenBorrowBook_thenDoesNotThrow() {
        when(databaseService.getUserById(userMock.getId())).thenReturn(userMock);
        when(databaseService.getBookByISBN(bookMock.getISBN())).thenReturn(bookMock);
        assertDoesNotThrow(() -> library.borrowBook(bookMock.getISBN(), userMock.getId()));
        verify(bookMock, atLeastOnce()).borrow();
        verify(bookMock, atLeastOnce()).isBorrowed();
        verify(databaseService, atLeastOnce()).borrowBook(bookMock.getISBN(), userMock.getId());
    }

    @Test
    public void givenUserNotRegistered_whenBorrowBook_thenUserNotRegisteredException() {

        when(databaseService.getUserById(userMock.getId())).thenReturn(null);
        when(databaseService.getBookByISBN(bookMock.getISBN())).thenReturn(bookMock);
        assertThrows(UserNotRegisteredException.class, () -> library.borrowBook(bookMock.getISBN(), userMock.getId()));
        verify(bookMock, never()).borrow();
        verify(databaseService, never()).borrowBook(bookMock.getISBN(), userMock.getId());
    }


    @Test
    void givenNullUser_whenRegisterUser_thenThrowIllegalArgumentException() {
        assertThrows(IllegalArgumentException.class, () -> library.registerUser(null));
    }

    @ParameterizedTest
    @ValueSource(strings = {
            "1234567890",
            "123-4567002",
            "0000",
            "123-0000000vb00A",
            "123ABCDFGHIJ",
            "123-12367890-12345",
            "123-4567",
            "123LALA-LO",
            "",
            " "

    })
    @NullSource
    void givenIllegalUserId_whenRegisterUser_thenThrowIllegalArgumentException(String id) {
        when(userMock.getId()).thenReturn(id);
        assertThrows(IllegalArgumentException.class, () -> library.registerUser(userMock));
        verify(userMock, atLeastOnce()).getId();
        verify(userMock, never()).getName();
        verify(userMock, never()).getNotificationService();
    }


    @ParameterizedTest
    @ValueSource(strings = {
            "",
    })
    @NullSource
    void givenEmptyOrNullUserName_whenRegisterUser_thenThrowIllegalArgumentException(String name) {
        when(userMock.getName()).thenReturn(name);
        assertThrows(IllegalArgumentException.class, () -> library.registerUser(userMock));
        verify(userMock, atLeastOnce()).getName();
        verify(userMock, never()).getNotificationService();
    }

    @Test
    void givenIllegalNotificationCenter_whenRegisterUser_thenThrowIllegalArgumentException() {
        when(userMock.getNotificationService()).thenReturn(null);
        assertThrows(IllegalArgumentException.class, () -> library.registerUser(userMock));
        verify(userMock, atLeastOnce()).getId();
        verify(userMock, atLeastOnce()).getName();
        verify(userMock, never()).getNotificationService();
    }

    @Test
    void givenExistingUser_whenRegisterUser_thenThrowIllegalArgumentException() {
        when(userMock.getNotificationService()).thenReturn(notificationService);
        when(userMock.getName()).thenReturn("Gal");
        when(databaseService.getUserById(userMock.getId())).thenReturn(userMock);
        assertThrows(IllegalArgumentException.class, () -> library.registerUser(userMock));
        verify(userMock, atLeastOnce()).getId();
        verify(userMock, atLeastOnce()).getName();
        verify(userMock, atLeastOnce()).getNotificationService();
        verify(databaseService, atLeastOnce()).getUserById(userMock.getId());
    }

    @Test
    public void givenValidUser_whenRegisterUser_thenDoesNotThrow() {
        when(userMock.getName()).thenReturn("Gal");
        when(userMock.getNotificationService()).thenReturn(notificationService);
        assertDoesNotThrow(() -> library.registerUser(userMock));
        verify(userMock, atLeastOnce()).getId();
        verify(userMock, atLeastOnce()).getName();
        verify(userMock, atLeastOnce()).getNotificationService();
    }


    @ParameterizedTest
    @ValueSource(strings = {
            "1234567890123",
            "123-456789002",
            "123-000000000000",
            "123-0000000vb00A",
            "123-ABCDE-FGHIJ",
            "123-12345-67890-12345",
            "123-4567",
            "123-LALA-LO"
    })
    @NullSource
    void givenNotValidISBN_whenReturnBook_thenIllegalArgumentException(String isbn) {
        assertThrows(IllegalArgumentException.class, () -> library.returnBook(isbn));
        verify(bookMock, never()).isBorrowed();
        verify(bookMock, never()).returnBook();
        verify(databaseService, never()).returnBook(isbn);
    }


    @Test
    void givenNotFoundBook_whenReturnBook_BookNotFoundException() {
        when(databaseService.getBookByISBN(bookMock.getISBN())).thenReturn(null);
        assertThrows(BookNotFoundException.class, () -> library.returnBook(bookMock.getISBN()));
        verify(bookMock, never()).isBorrowed();
        verify(bookMock, never()).returnBook();
        verify(databaseService, never()).returnBook(bookMock.getISBN());
    }

    @ParameterizedTest
    @ValueSource(strings = {
            "1234567890123",
            "123-456789002",
            "123-000000000000",
            "123-0000000vb00A",
            "123-ABCDE-FGHIJ",
            "123-12345-67890-12345",
            "123-4567",
            "123-LALA-LO"
    })
    public void givenValidISBNOfNonBorrowedBook_whenReturnBook_thenBookNotBorrowedException() {

        when(bookMock.isBorrowed()).thenReturn(false);
        when(databaseService.getBookByISBN(bookMock.getISBN())).thenReturn(bookMock);
        assertThrows(BookNotBorrowedException.class, () -> library.returnBook(bookMock.getISBN()));
        verify(bookMock, atLeastOnce()).isBorrowed();
        verify(bookMock, never()).returnBook();
        verify(databaseService, never()).returnBook(bookMock.getISBN());
    }

    @Test
    public void givenValidISBN_whenReturnBook_thenDoesNotThrow() {
        when(bookMock.isBorrowed()).thenReturn(true);
        when(databaseService.getBookByISBN(bookMock.getISBN())).thenReturn(bookMock);
        assertDoesNotThrow(() -> library.returnBook(bookMock.getISBN()));
        verify(bookMock, atLeastOnce()).isBorrowed();
        verify(bookMock).returnBook();
        verify(databaseService).returnBook(bookMock.getISBN());
    }


    @ParameterizedTest
    @ValueSource(strings = {
            "1234567890123",
            "123-456789002",
            "123-000000000000",
            "123-0000000vb00A",
            "123-ABCDE-FGHIJ",
            "123-12345-67890-12345",
            "123-4567",
            "123-LALA-LO"
    })
    @NullSource
    void givenNotValidISBN_whenNotifyUserWithBookReviews_thenThrowException(String isbn) {
        when(bookMock.getISBN()).thenReturn(isbn);
        assertThrows(IllegalArgumentException.class, () -> library.notifyUserWithBookReviews(bookMock.getISBN(), userMock.getId()));
        verify(reviewService, never()).getReviewsForBook(bookMock.getISBN());
        verify(reviewService, never()).close();
    }


    @ParameterizedTest
    @ValueSource(strings = {
            "123-0000000vb00A",
            "123ABCDFGHIJ",
            "123LALA-LO",
            "",
            " "

    })
    void givenNotValidUserIdLetters_whenNotifyUserWithBookReviews_thenThrowIllegalArgumentException(String id) {
        when(userMock.getId()).thenReturn(id);
        assertThrows(IllegalArgumentException.class, () -> library.notifyUserWithBookReviews(bookMock.getISBN(), userMock.getId()));
        verify(reviewService, never()).getReviewsForBook(bookMock.getISBN());
        verify(reviewService, never()).close();
    }

    @ParameterizedTest
    @ValueSource(strings = {
            "12300000002344433",
            "1232345678990",
            "123435346565678678"
    })
    void givenNotValidUserIdTooLong_whenNotifyUserWithBookReviews_thenThrowIllegalArgumentException(String id) {
        when(userMock.getId()).thenReturn(id);
        assertThrows(IllegalArgumentException.class, () -> library.notifyUserWithBookReviews(bookMock.getISBN(), userMock.getId()));
        verify(reviewService, never()).getReviewsForBook(bookMock.getISBN());
        verify(reviewService, never()).close();
    }

    @ParameterizedTest
    @ValueSource(strings = {
            "12334@@@@",
            "@@@@@@@@@@@@",
            "123435@!@$$"
    })
    void givenNotValidUserIdSpecialChars_whenNotifyUserWithBookReviews_thenThrowIllegalArgumentException(String id) {
        when(userMock.getId()).thenReturn(id);
        assertThrows(IllegalArgumentException.class, () -> library.notifyUserWithBookReviews(bookMock.getISBN(), userMock.getId()));
        verify(reviewService, never()).getReviewsForBook(bookMock.getISBN());
        verify(reviewService, never()).close();
    }

    @Test
    void givenNotFoundBook_whenNotifyUserWithBookReviews_BookNotFoundException() {
        when(databaseService.getBookByISBN(bookMock.getISBN())).thenReturn(null);
        assertThrows(BookNotFoundException.class, () -> library.notifyUserWithBookReviews(bookMock.getISBN(), userMock.getId()));
        verify(reviewService, never()).getReviewsForBook(bookMock.getISBN());
        verify(reviewService, never()).close();
    }

    @Test
    public void givenUserNotInLibrary_whenNotifyUserWithBookReviews_thenUserNotRegisteredException() {
        when(databaseService.getBookByISBN(bookMock.getISBN())).thenReturn(bookMock);
        when(databaseService.getUserById(userMock.getId())).thenReturn(null);
        assertThrows(UserNotRegisteredException.class, () -> library.notifyUserWithBookReviews(bookMock.getISBN(), userMock.getId()));
    }


    @ParameterizedTest
    @NullSource
    @EmptySource
    public void givenIllegalReviews_whenNotifyUserWithBookReviews_thenNoReviewsFoundException(List<String> reviews) {
        when(databaseService.getBookByISBN(bookMock.getISBN())).thenReturn(bookMock);
        when(databaseService.getUserById(userMock.getId())).thenReturn(userMock);
        when(reviewService.getReviewsForBook(bookMock.getISBN())).thenReturn(reviews);
        assertThrows(NoReviewsFoundException.class, () -> library.notifyUserWithBookReviews(bookMock.getISBN(), userMock.getId()));
        verify(reviewService, atLeastOnce()).getReviewsForBook(bookMock.getISBN());
        verify(reviewService).close();
    }

    @Test
    public void givenReviewException_whenNotifyUserWithBookReviews_thenReviewServiceUnavailableException() {

        when(databaseService.getBookByISBN(bookMock.getISBN())).thenReturn(bookMock);
        when(databaseService.getUserById(userMock.getId())).thenReturn(userMock);
        when(reviewService.getReviewsForBook(bookMock.getISBN())).thenThrow(ReviewException.class);
        assertThrows(ReviewServiceUnavailableException.class, () -> library.notifyUserWithBookReviews(bookMock.getISBN(), userMock.getId()));
        verify(reviewService, atLeastOnce()).getReviewsForBook(bookMock.getISBN());
        verify(reviewService).close();
    }

    @Test
    public void givenValidInfoToUnavailableReviewService_whenNotifyUserWithBookReviews_thenNotificationException() {
        reviews.add("Bad");
        reviews.add("Horrible");
        String notification_Message = "Reviews for '" + bookMock.getTitle() + "':\n" + String.join("\n", reviews);

        when(databaseService.getBookByISBN(bookMock.getISBN())).thenReturn(bookMock);
        when(databaseService.getUserById(userMock.getId())).thenReturn(userMock);
        when(reviewService.getReviewsForBook(bookMock.getISBN())).thenReturn(reviews);
        when(databaseService.getUserById(userMock.getId())).thenReturn(userMock);
        doThrow(NotificationException.class).when(userMock).sendNotification(notification_Message);
        assertThrows(NotificationException.class, () -> library.notifyUserWithBookReviews(bookMock.getISBN(), userMock.getId()));
        verify(userMock, atLeastOnce()).sendNotification("Reviews for '" + bookMock.getTitle() + "':\n" + String.join("\n", reviews));
        verify(reviewService, atLeastOnce()).getReviewsForBook(bookMock.getISBN());
        verify(reviewService).close();
    }

    @Test
    public void givenReviews_whenNotifyUserWithBookReviews_thenDoesNotThrow() { //TODO: change test

        when(databaseService.getBookByISBN(bookMock.getISBN())).thenReturn(bookMock);
        when(databaseService.getUserById(userMock.getId())).thenReturn(userMock);
        List<String> reviews = new ArrayList<>();
        reviews.add("Bad");
        reviews.add("horrible");
        when(reviewService.getReviewsForBook(bookMock.getISBN())).thenReturn(reviews);
        assertDoesNotThrow(() -> library.notifyUserWithBookReviews(bookMock.getISBN(), userMock.getId()));
        verify(userMock, atLeastOnce()).sendNotification("Reviews for '" + bookMock.getTitle() + "':\n" + String.join("\n", reviews));
        verify(reviewService, atLeastOnce()).getReviewsForBook(bookMock.getISBN());
        verify(reviewService).close();
    }


    @Test
    public void givenValidISBN_whenGetBookByISBN_DoesNotThrow() {
        when(bookMock.isBorrowed()).thenReturn(false);
        when(databaseService.getBookByISBN(bookMock.getISBN())).thenReturn(bookMock);
        assertDoesNotThrow(() -> library.getBookByISBN(bookMock.getISBN(), userMock.getId()));
        verify(bookMock, atLeastOnce()).isBorrowed();
    }


    @ParameterizedTest
    @CsvSource({
            "1234567890123",
            "123-456789002",
            "123-000000000000",
            "123-0000000vb00A",
            "123-ABCDE-FGHIJ",
            "123-12345-67890-12345",
            "123-4567",
            "123-LALA-LO"
    })
    @NullSource
    void givenIllegalISBN_whenGetBookByISBN_thenIllegalArgumentException(String isbn) {
        when(bookMock.isBorrowed()).thenReturn(false);
        assertThrows(IllegalArgumentException.class, () -> library.getBookByISBN(isbn, userMock.getId()));
        verify(bookMock, never()).isBorrowed();
    }

    @Test
    public void givenValidISBNOfNonExistingBook_whenGetBookByISBN_thenBookNotFoundException() {
        when(bookMock.isBorrowed()).thenReturn(false);
        when(databaseService.getBookByISBN(bookMock.getISBN())).thenReturn(null);
        assertThrows(BookNotFoundException.class, () -> library.getBookByISBN(bookMock.getISBN(), userMock.getId()));
        verify(bookMock, never()).isBorrowed();
    }

    @ParameterizedTest
    @ValueSource(strings = {
            "1234567890",
            "123-4567002",
            "0000",
            "123-0000000vb00A",
            "123ABCDFGHIJ",
            "123-12367890-12345",
            "123-4567",
            "123LALA-LO",
            "",
            " "

    })
    @NullSource
    public void givenInvalidUserID_whenGetBookByISBN_thenIllegalArgumentException(String id) {
        when(bookMock.isBorrowed()).thenReturn(false);
        when(databaseService.getBookByISBN(bookMock.getISBN())).thenReturn(bookMock);
        assertThrows(IllegalArgumentException.class, () -> library.getBookByISBN(bookMock.getISBN(), id));
        verify(bookMock, never()).isBorrowed();
    }


    @Test
    public void givenValidISBNAndValidUser_whenGetBookByISBN_thenDoesNotThrow() {
        when(bookMock.isBorrowed()).thenReturn(false);
        when(databaseService.getBookByISBN(bookMock.getISBN())).thenReturn(bookMock);
        when(databaseService.getUserById(userMock.getId())).thenReturn(userMock);

        assertDoesNotThrow(() -> library.getBookByISBN(bookMock.getISBN(), userMock.getId()));

        verify(databaseService, times(2)).getBookByISBN(bookMock.getISBN()); // Expect two calls
        verify(bookMock, atLeastOnce()).isBorrowed();
        verify(databaseService).getUserById(userMock.getId());
    }



    @Test
    public void givenBorrowedBook_whenGetBookByISBN_thenBookAlreadyBorrowedException() {
        when(bookMock.isBorrowed()).thenReturn(true); 
        when(databaseService.getBookByISBN(bookMock.getISBN())).thenReturn(bookMock);
        assertThrows(BookAlreadyBorrowedException.class,
                () -> library.getBookByISBN(bookMock.getISBN(), userMock.getId()));
        verify(databaseService).getBookByISBN(bookMock.getISBN()); 
        verify(bookMock, atLeastOnce()).isBorrowed(); 
        verify(databaseService, never()).getUserById(anyString()); 
    }


    @ParameterizedTest
    @ValueSource(strings = {
            "1234567890",
            "123-4567002",
            "0000",
            "123-0000000vb00A",
            "123ABCDFGHIJ",
            "123-12367890-12345",
            "123-4567",
            "123LALA-LO",
            "",
            " "
    })
    @NullSource
    public void givenInvalidUserId_whenGetBookByISBN_thenIllegalArgumentException(String id) {
        when(databaseService.getBookByISBN(bookMock.getISBN())).thenReturn(bookMock);

        assertThrows(IllegalArgumentException.class,
                () -> library.getBookByISBN(bookMock.getISBN(), id));

        verify(databaseService, never()).getBookByISBN(anyString());
        verify(databaseService, never()).getUserById(anyString());
    }

    @Test
    public void givenNonExistentBook_whenGetBookByISBN_thenBookNotFoundException() {
        when(databaseService.getBookByISBN(bookMock.getISBN())).thenReturn(null); // Book doesn't exist

        assertThrows(BookNotFoundException.class, () -> library.getBookByISBN(bookMock.getISBN(), userMock.getId()));

        verify(databaseService).getBookByISBN(bookMock.getISBN()); // Ensure only this is called
        verify(databaseService, never()).getUserById(anyString()); // No user lookup
    }


    @Test
    public void givenNullUser_whenGetBookByISBN_thenIllegalArgumentException() {
        when(bookMock.isBorrowed()).thenReturn(false);
        when(databaseService.getBookByISBN(bookMock.getISBN())).thenReturn(bookMock);
        when(databaseService.getUserById(null)).thenReturn(null);

        assertThrows(IllegalArgumentException.class,
                () -> library.getBookByISBN(bookMock.getISBN(), null));

        verify(databaseService, never()).getBookByISBN(anyString());
        verify(databaseService, never()).getUserById(anyString());
        verify(bookMock, never()).isBorrowed();
    }

    @Test
    public void givenValidISBNAndUserId_whenGetBookByISBNButNotificationFails_thenDoesNotThrow() {
        when(bookMock.isBorrowed()).thenReturn(false);
        when(databaseService.getBookByISBN(bookMock.getISBN())).thenReturn(bookMock);
        when(databaseService.getUserById(userMock.getId())).thenReturn(userMock);
        doThrow(NotificationException.class).when(userMock).sendNotification(anyString());

        assertDoesNotThrow(() -> library.getBookByISBN(bookMock.getISBN(), userMock.getId()));

        verify(databaseService, times(2)).getBookByISBN(bookMock.getISBN());
        verify(bookMock, atLeastOnce()).isBorrowed();
        verify(databaseService).getUserById(userMock.getId());
    }



}
