This is a project I did for my Entity Framework Teamwork assignment. I was assigned with two other people, but after trying to contact them in several ways, they showed no interest so I did it all by myself. I think that this project was far more complex than my first one since even though it was all strings and numbers and not some heavy math it ended up far more clean and user friendly. What I wanted to achieve with some of my decisions is not having the user stuck on a screen and not knowing what to do. 
I’ve used a lot of what I learned in the EF module from SoftUni. I’ve constantly used migrations while developing the data base. Even though it was supposed to be a betting and gambling application most of my time was spent on the actual logged user – not logged user – admin interactions. The math behind the project is really simple and all it involves are some basic calculations in order to proceed all the data that is input in the DB, however it has several user roles which all have their corresponding functionality and ways to “edit” the database and enter new data. I think it was a really fun project and although in the beginning seemed really simple I enjoyed having to overcome all the obstacles that came with all the user levels introduced in it.
In the admin menu there are options for exporting some of the data in xml files. Also there is an option for the admin to create a json template for entering the matches scores in the DB, for easier workflow.


# DB_Advanced_Teamwork
KhonsuBetManager

Initial WorkPlan (might be out of date):
https://docs.google.com/spreadsheets/d/1UyuyxH8NBWgrdTSEfoZDH7DbEv70JDdWkZG5gCFQIho/edit#gid=0

TODO:

User Create Functionality:<br/>
```diff
+Make **User** **Register** function -> **DONE**<br/>
+Make **User** **Login** function -> **DONE**<br/>
+Make **User** **Logout** function -> **DONE**<br/>
+Promote some **Users** to **Admins** manualy -> **DONE**<br/>
+Make **User** function to **Change Password** -> **DONE**<br/>
-Make **User** function to **Delete Account**<br/>
-Make **User** function to **Restore Account**<br/>
-Make **User** function to **Reset Password**<br/>
-send e-mail with random generated password (this will be added later if possible)<br/>
```
Admin Functionality:<br/>
```diff
+Make function for **Owners** to **Promote** other **Users** to **Admins** -> **DONE**<br/>
+Make function for **Owners** to **Demote** other **Users** from **Admins** -> **DONE**<br/>
+Make function for **Admin** to **Add Matches** -> **DONE**<br/>
+Make function for **Admin** to **Update Match Results** -> **DONE**<br/>
+Make function when **Match Score** is updated to **Resolve Bets** -> **DONE**<br/>
+Add initial money to operate with -> **DONE**<br/>
+Make function for **Admin** to export all user info in xml -> **DONE**<br/>
+Make function for **Admin** to **Add Matches from json** -> **DONE**<br/>
-Make **Admin** function to **Delete Account**<br/>
-Make **Admin** function to **Restore Account**<br/>
-Make some **Accounting** functionality for **Admin/Owner** -> print some data in xml or json<br/>
```
User Betting Functionality:<br/>
```diff
+Make function for **User** to **Deposit Money** -> **DONE**<br/>
+Make function for **User** to **Withdraw Money** -> **DONE**<br/>
+Make function for **User** to **Place Bets** --> **DONE**<br/>
+Make function for **User** to **View Matches** -> **DONE**<br/>
+Make function for **User** to **View User Info (money and stuff)** ->**DONE**<br/>
+Make function for **User** to view **Placed Bets** -> **DONE**<br/>
-Make function for **User** to **Filter Matches**<br/>
-Display resolved matches since last Login on **User** **Login**<br/>
```
General:<br/>
```diff
+Add **HELP** Menu<br/>
-**Add more functions**<br/>
```

