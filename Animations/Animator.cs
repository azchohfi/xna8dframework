using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace XNA8DFramework.Animations
{
	public class Animator : GameComponent
	{
		protected List<Animation> Animacoes;
		protected Queue<Animation> AnimacoesQueue;

		private bool _autoRemove;

		public bool AutoRemove
		{
			get { return _autoRemove; }
			set { _autoRemove = value; }
		}

		public Animator(Game game)
			: base(game)
		{
			Animacoes = new List<Animation>();
			AnimacoesQueue = new Queue<Animation>();
		}

		public void AddAnimation(Animation animation)
		{
			AddAnimation(animation, false);
		}

		public void AddAnimation(Animation animation, bool start)
		{
			if (start)
				animation.Start();
			AnimacoesQueue.Enqueue(animation);
		}

		public void RemoveAnimation(Animation animation)
		{
			Animacoes.Remove(animation);

			var resAnimacoesQueue = AnimacoesQueue.Where(a => a == animation);
			if (resAnimacoesQueue.Any())
				AnimacoesQueue = new Queue<Animation>(AnimacoesQueue.Except(resAnimacoesQueue));
		}

		public override void Update(GameTime gameTime)
		{
			if (!Enabled)
				return;

			AddFromQueue();

			for (int i = 0; i < Animacoes.Count; i++)
			{
				if (Animacoes[i].Terminou)
					continue;
				Animacoes[i].Update(gameTime);
				Animacoes[i].Update2(gameTime);
			}

			if (_autoRemove)
			{
				for (int i = Animacoes.Count - 1; i >= 0; i--)
				{
					if (Animacoes[i].Terminou)
						Animacoes.RemoveAt(i);
				}
			}

			base.Update(gameTime);
		}

		private void AddFromQueue()
		{
			var queueAnimCount = AnimacoesQueue.Count;
			for (var i = 0; i < queueAnimCount; i++)
				Animacoes.Add(AnimacoesQueue.Dequeue());
		}

		public void Start()
		{
			AddFromQueue();
			foreach (Animation animacao in Animacoes)
			{
				animacao.Start();
			}
		}

		public void Clear()
		{
			AnimacoesQueue.Clear();
			Animacoes.Clear();
		}

		public void RemoveAllFrom(object animatable)
		{
#if WINDOWS
			Animacoes.RemoveAll(a => a.AnimatableG == animatable);
#else
			var resAnimacoes = Animacoes.Where(a => a.AnimatableG == animatable);
			if (resAnimacoes.Any())
				Animacoes = new List<Animation>(Animacoes.Except(resAnimacoes));
#endif

			var resAnimacoesQueue = AnimacoesQueue.Where(a => a.AnimatableG == animatable);
			if (resAnimacoesQueue.Any())
				AnimacoesQueue = new Queue<Animation>(AnimacoesQueue.Except(resAnimacoesQueue));
		}

		public int Count()
		{
			return Animacoes.Count;
		}

		#region Easing Curves
		/*
		private float linear(float start, float end, float value){
			return Mathf.Lerp(start, end, value);
		}
	
		private float clerp(float start, float end, float value){
			float min = 0.0f;
			float max = 360.0f;
			float half = Mathf.Abs((max - min) / 2.0f);
			float retval = 0.0f;
			float diff = 0.0f;
			if ((end - start) < -half){
				diff = ((max - start) + end) * value;
				retval = start + diff;
			}else if ((end - start) > half){
				diff = -((max - end) + start) * value;
				retval = start + diff;
			}else retval = start + (end - start) * value;
			return retval;
		}

		private float spring(float start, float end, float value){
			value = Mathf.Clamp01(value);
			value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
			return start + (end - start) * value;
		}

		private float easeInQuad(float start, float end, float value){
			end -= start;
			return end * value * value + start;
		}

		private float easeOutQuad(float start, float end, float value){
			end -= start;
			return -end * value * (value - 2) + start;
		}

		private float easeInOutQuad(float start, float end, float value){
			value /= .5f;
			end -= start;
			if (value < 1) return end / 2 * value * value + start;
			value--;
			return -end / 2 * (value * (value - 2) - 1) + start;
		}

		private float easeInCubic(float start, float end, float value){
			end -= start;
			return end * value * value * value + start;
		}

		private float easeOutCubic(float start, float end, float value){
			value--;
			end -= start;
			return end * (value * value * value + 1) + start;
		}

		private float easeInOutCubic(float start, float end, float value){
			value /= .5f;
			end -= start;
			if (value < 1) return end / 2 * value * value * value + start;
			value -= 2;
			return end / 2 * (value * value * value + 2) + start;
		}

		private float easeInQuart(float start, float end, float value){
			end -= start;
			return end * value * value * value * value + start;
		}

		private float easeOutQuart(float start, float end, float value){
			value--;
			end -= start;
			return -end * (value * value * value * value - 1) + start;
		}

		private float easeInOutQuart(float start, float end, float value){
			value /= .5f;
			end -= start;
			if (value < 1) return end / 2 * value * value * value * value + start;
			value -= 2;
			return -end / 2 * (value * value * value * value - 2) + start;
		}

		private float easeInQuint(float start, float end, float value){
			end -= start;
			return end * value * value * value * value * value + start;
		}

		private float easeOutQuint(float start, float end, float value){
			value--;
			end -= start;
			return end * (value * value * value * value * value + 1) + start;
		}

		private float easeInOutQuint(float start, float end, float value){
			value /= .5f;
			end -= start;
			if (value < 1) return end / 2 * value * value * value * value * value + start;
			value -= 2;
			return end / 2 * (value * value * value * value * value + 2) + start;
		}

		private float easeInSine(float start, float end, float value){
			end -= start;
			return -end * Mathf.Cos(value / 1 * (Mathf.PI / 2)) + end + start;
		}

		private float easeOutSine(float start, float end, float value){
			end -= start;
			return end * Mathf.Sin(value / 1 * (Mathf.PI / 2)) + start;
		}

		private float easeInOutSine(float start, float end, float value){
			end -= start;
			return -end / 2 * (Mathf.Cos(Mathf.PI * value / 1) - 1) + start;
		}

		private float easeInExpo(float start, float end, float value){
			end -= start;
			return end * Mathf.Pow(2, 10 * (value / 1 - 1)) + start;
		}

		private float easeOutExpo(float start, float end, float value){
			end -= start;
			return end * (-Mathf.Pow(2, -10 * value / 1) + 1) + start;
		}

		private float easeInOutExpo(float start, float end, float value){
			value /= .5f;
			end -= start;
			if (value < 1) return end / 2 * Mathf.Pow(2, 10 * (value - 1)) + start;
			value--;
			return end / 2 * (-Mathf.Pow(2, -10 * value) + 2) + start;
		}

		private float easeInCirc(float start, float end, float value){
			end -= start;
			return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
		}

		private float easeOutCirc(float start, float end, float value){
			value--;
			end -= start;
			return end * Mathf.Sqrt(1 - value * value) + start;
		}

		private float easeInOutCirc(float start, float end, float value){
			value /= .5f;
			end -= start;
			if (value < 1) return -end / 2 * (Mathf.Sqrt(1 - value * value) - 1) + start;
			value -= 2;
			return end / 2 * (Mathf.Sqrt(1 - value * value) + 1) + start;
		}

		private float easeInBounce(float start, float end, float value){
			end -= start;
			float d = 1f;
			return end - easeOutBounce(0, end, d-value) + start;
		}

		private float easeOutBounce(float start, float end, float value){
			value /= 1f;
			end -= start;
			if (value < (1 / 2.75f)){
				return end * (7.5625f * value * value) + start;
			}else if (value < (2 / 2.75f)){
				value -= (1.5f / 2.75f);
				return end * (7.5625f * (value) * value + .75f) + start;
			}else if (value < (2.5 / 2.75)){
				value -= (2.25f / 2.75f);
				return end * (7.5625f * (value) * value + .9375f) + start;
			}else{
				value -= (2.625f / 2.75f);
				return end * (7.5625f * (value) * value + .984375f) + start;
			}
		}

		private float easeInOutBounce(float start, float end, float value){
			end -= start;
			float d = 1f;
			if (value < d/2) return easeInBounce(0, end, value*2) * 0.5f + start;
			else return easeOutBounce(0, end, value*2-d) * 0.5f + end*0.5f + start;
		}

		private float easeInBack(float start, float end, float value){
			end -= start;
			value /= 1;
			float s = 1.70158f;
			return end * (value) * value * ((s + 1) * value - s) + start;
		}

		private float easeOutBack(float start, float end, float value){
			float s = 1.70158f;
			end -= start;
			value = (value / 1) - 1;
			return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
		}

		private float easeInOutBack(float start, float end, float value){
			float s = 1.70158f;
			end -= start;
			value /= .5f;
			if ((value) < 1){
				s *= (1.525f);
				return end / 2 * (value * value * (((s) + 1) * value - s)) + start;
			}
			value -= 2;
			s *= (1.525f);
			return end / 2 * ((value) * value * (((s) + 1) * value + s) + 2) + start;
		}

		private float punch(float amplitude, float value){
			float s = 9;
			if (value == 0){
				return 0;
			}
			if (value == 1){
				return 0;
			}
			float period = 1 * 0.3f;
			s = period / (2 * Mathf.PI) * Mathf.Asin(0);
			return (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
		}
	
		private float easeInElastic(float start, float end, float value){
			end -= start;
		
			float d = 1f;
			float p = d * .3f;
			float s = 0;
			float a = 0;
		
			if (value == 0) return start;
		
			if ((value /= d) == 1) return start + end;
		
			if (a == 0f || a < Mathf.Abs(end)){
				a = end;
				s = p / 4;
				}else{
				s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
			}
		
			return -(a * Mathf.Pow(2, 10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
		}		

		private float easeOutElastic(float start, float end, float value){
			//Thank you to rafael.marteleto for fixing this as a port over from Pedro's UnityTween
			end -= start;
		
			float d = 1f;
			float p = d * .3f;
			float s = 0;
			float a = 0;
		
			if (value == 0) return start;
		
			if ((value /= d) == 1) return start + end;
		
			if (a == 0f || a < Mathf.Abs(end)){
				a = end;
				s = p / 4;
				}else{
				s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
			}
		
			return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
		}		
	
		private float easeInOutElastic(float start, float end, float value){
			end -= start;
		
			float d = 1f;
			float p = d * .3f;
			float s = 0;
			float a = 0;
		
			if (value == 0) return start;
		
			if ((value /= d/2) == 2) return start + end;
		
			if (a == 0f || a < Mathf.Abs(end)){
				a = end;
				s = p / 4;
				}else{
				s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
			}
		
			if (value < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
			return a * Mathf.Pow(2, -10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
		}		
		*/
		#endregion
	}
}
