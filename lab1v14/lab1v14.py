class Restaurant:
    def __init__(self, name: str, cuisine: str, rating: float):
        self.name = name
        self.cuisine = cuisine
        self._rating = rating
        print(f"Ресторан {self.name} відкрито.")
    @property
    def rating(self) -> float:
        return self._rating
    @rating.setter
    def rating(self, value: float):
        if 0 <= value <= 5:
            self._rating = value
        else:
            print("Рейтинг має бути від 0 до 5.")
    def serve_dish(self, dish: str) -> str:
        return f"{self.name} подає страву: {dish} ({self.cuisine} кухня)."

    def __del__(self):
        print(f"Ресторан {self.name} закрито.")
def main():
    r1 = Restaurant("Italia Bella", "Італійська", 4.5)
    r2 = Restaurant("Sakura", "Японська", 4.8)
    r3 = Restaurant("Borshch", "Українська", 4.2)
    print(r1.serve_dish("Піцца Маргарита"))
    print(r2.serve_dish("Суші"))
    print(r3.serve_dish("Борщ"))
    print(f"Рейтинг {r1.name}: {r1.rating}")
    r1.rating = 5
    print(f"Новий рейтинг {r1.name}: {r1.rating}")


if __name__ == "__main__":
    main()