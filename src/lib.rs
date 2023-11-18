use std::slice;
use std::str;
use tts::Tts;

pub struct State {
    tts: Tts,
}

#[no_mangle]
pub extern "C" fn create() -> *mut State {
    // Create a new Text-To-Speech instance
    match Tts::default() {
        Ok(tts) => {
            let state = Box::new(State { tts });
            Box::into_raw(state)
        }
        Err(_) => std::ptr::null_mut(),
    }
}

#[no_mangle]
pub extern "C" fn speak(state: *mut State, speed: f32, text_ptr: *const u8, text_len: i32) -> u32 {
    let state = unsafe { &mut *state };
    let tts = &mut state.tts;

    let slice = unsafe { slice::from_raw_parts(text_ptr, text_len as usize) };
    let text = str::from_utf8(slice).unwrap();

    _ = tts.set_rate(speed);

    match tts.speak(text, true) {
        Ok(_) => 1,
        Err(_) => 0,
    }
}

#[no_mangle]
pub extern "C" fn is_playing(state: *mut State) -> u32 {
    let state = unsafe { &mut *state };
    match state.tts.is_speaking() {
        Ok(b) => {
            if b {
                1
            } else {
                0
            }
        }
        Err(_) => 0,
    }
}

#[no_mangle]
pub extern "C" fn destroy(state: *mut State) {
    unsafe {
        let _ = Box::from_raw(state);
    }
}
