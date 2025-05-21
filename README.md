# Plagiarism checker  

### Мета проєкту

**Plagiarism checker** - це pet-проєкт, ціль якого ознайомитись і засвоїти різні практики та підходи у створені веб застосунків
на платформі .Net Core та фреймворку Angular 2. Проект еволюціонує з версії до версії, отримуючи більш широкий спектор різноманітних технологій та рішень,
притаманних сучасній комерційній розробці. Цей проект охоплює повний життєвий цикл створення програмного забезпеченння такого роду
застосунків. Веб-додаток має повністю функціональну `front-end` та `back-end` частини. Усі зовнішні сервіси піднімаються в докері з
використанням оркестрації docker-compose. Покриття тестами становить більше ніж 90%.

### Опис проєкту
Застосунок моделює роботу платформи перевірки студентських робіт на плагіат. Plagiarism checker містить в собі систему
авторизації та автентифікації, можливість завантаження та перегляду робіт а також беспосередньо сама перевірка на плагіат. 

## Архітектура

Для опису архітектури було використано [C4 Model Diagram](https://c4model.com/), яка за допомогою шарів **System Context**, **Container**, **Component**
та **Code** дає розробникам та зацікавленим сторонам уявлення, як працює система.

#### C1 System Context

![](https://github.com/Nazg0r/.NET-labs/blob/lab3/Docs/Images/C1-diagram.png)

#### C2 Container

![](https://github.com/Nazg0r/.NET-labs/blob/lab3/Docs/Images/C2-diagream.png)

#### C3 Component

![](https://github.com/Nazg0r/.NET-labs/blob/lab3/Docs/Images/C3-diagream.png)

#### UML component diagram:

![](https://github.com/Nazg0r/.NET-labs/blob/lab3/Docs/Images/UML-components.png)

#### Deployment diagram

![](https://github.com/Nazg0r/.NET-labs/blob/lab3/Docs/Images/deployment-diagram.png)
