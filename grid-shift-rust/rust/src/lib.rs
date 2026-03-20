use godot::prelude::*;

pub mod game_state;
pub mod grid_pos;
pub mod level_catalog;
pub mod level_loader;
pub mod level_runtime;
pub mod rust_game_controller;

#[cfg(test)]
mod logic_tests;

struct GridShiftRustExtension;

#[gdextension]
unsafe impl ExtensionLibrary for GridShiftRustExtension {}

#[derive(GodotClass)]
#[class(base = Node)]
struct RustProbe {
    base: Base<Node>,
}

#[godot_api]
impl INode for RustProbe {
    fn init(base: Base<Node>) -> Self {
        Self { base }
    }

    fn ready(&mut self) {
        godot_print!("RustProbe ready: gdext loaded successfully");
        let _ = &self.base;
    }
}
