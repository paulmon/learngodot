use std::ops::Add;

#[derive(Debug, Clone, Copy, PartialEq, Eq, Hash)]
pub struct GridPos {
    pub x: i32,
    pub y: i32,
}

impl GridPos {
    pub const ZERO: Self = Self { x: 0, y: 0 };
    pub const UP: Self = Self { x: 0, y: -1 };
    pub const DOWN: Self = Self { x: 0, y: 1 };
    pub const LEFT: Self = Self { x: -1, y: 0 };
    pub const RIGHT: Self = Self { x: 1, y: 0 };

    pub const fn new(x: i32, y: i32) -> Self {
        Self { x, y }
    }
}

impl Add for GridPos {
    type Output = GridPos;

    fn add(self, rhs: Self) -> Self::Output {
        GridPos {
            x: self.x + rhs.x,
            y: self.y + rhs.y,
        }
    }
}
