mergeInto(LibraryManager.library, {
  OnSoundPauseComplete: function() {
    if (typeof window.OnSoundPauseComplete === 'function') {
      window.OnSoundPauseComplete();
    } else {
      console.error('OnSoundPauseComplete function is not defined.');
    }
  },
  OnSoundResumeComplete: function() {
    if (typeof window.OnSoundResumeComplete === 'function') {
      window.OnSoundResumeComplete();
    } else {
      console.error('OnSoundResumeComplete function is not defined.');
    }
  },
  AlreadyMounted: function() {
    if (typeof window.AlreadyMounted === 'function') {
      window.AlreadyMounted();
    } else {
      console.error('AlreadyMounted function is not defined.');
    }
  },
  SetPluginVersion: function() {
    window.pluginVersion = "1";
  }
});